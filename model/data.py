import os
import json
import openai
import chromadb
from chromadb.utils import embedding_functions
from typing import List

# OpenAI API Key 설정
openai.api_key = os.getenv("OPENAI_API_KEY")
if not openai.api_key:
    raise EnvironmentError("OpenAI API Key가 설정되지 않았습니다. 환경 변수 'OPENAI_API_KEY'를 설정하세요.")

# ChromaDB 환경 변수 설정
os.environ["CHROMA_DB_IMPL"] = "duckdb+parquet"
os.environ["CHROMA_PERSIST_DIRECTORY"] = "./chroma_db"

class OpenAIEmbeddingFunction(embedding_functions.EmbeddingFunction):
    def __init__(self, model_name: str = "text-embedding-ada-002"):
        self.model_name = model_name

    def __call__(self, texts: List[str]) -> List[List[float]]:
        response = openai.Embedding.create(
            input=texts,
            engine=self.model_name
        )
        embeddings = [res["embedding"] for res in response["data"]]
        return embeddings

# ChromaDB Client 초기화
client = chromadb.Client()

# 임베딩 함수 생성
embedding_fn = OpenAIEmbeddingFunction()

# 데이터 디렉토리 설정
judgement_dir = "./sample/판결문/라벨링데이터"
clause_dir = "./sample/약관/라벨링데이터"

# 컬렉션 생성
judgement_collection = client.get_or_create_collection(
    name="judgement_docs",
    embedding_function=embedding_fn
)

clause_collection = client.get_or_create_collection(
    name="clause_docs",
    embedding_function=embedding_fn
)

def validate_and_prepare_metadata(metadata: dict) -> dict:
    """
    메타데이터를 검증하고 JSON 직렬화 가능한 형태로 변환
    """
    new_metadata = {}
    for k, v in metadata.items():
        if isinstance(v, (list, dict)):
            new_metadata[k] = json.dumps(v, ensure_ascii=False)  # JSON 문자열로 변환
        elif isinstance(v, (str, int, float, bool)):
            new_metadata[k] = v
        else:
            new_metadata[k] = str(v)  # 기타 데이터 타입은 문자열로 변환
    return new_metadata

def process_judgement_data(data: dict) -> (str, dict):
    """
    판결문 데이터를 처리하여 텍스트와 메타데이터로 변환
    """
    text_parts = data.get("facts", {}).get("bsisFacts", []) + \
                 data.get("dcss", {}).get("courtDcss", []) + \
                 data.get("close", {}).get("cnclsns", [])
    document_text = "\n".join(text_parts)
    metadata = {k: v for k, v in data.items() if k not in ["facts", "dcss", "close"]}
    metadata["mediaType"] = "판결문"
    return document_text, validate_and_prepare_metadata(metadata)

def process_clause_data(data: dict) -> (str, dict):
    """
    약관 데이터를 처리하여 텍스트와 메타데이터로 변환
    """
    text_parts = data.get("clauseArticle", []) + \
                 data.get("illdcssBasiss", []) + \
                 data.get("relateLaword", [])
    document_text = "\n".join(text_parts)
    metadata = {k: v for k, v in data.items() if k not in ["clauseArticle", "illdcssBasiss", "relateLaword"]}
    metadata["mediaType"] = "약관"
    return document_text, validate_and_prepare_metadata(metadata)

def process_file(file_path: str, collection):
    with open(file_path, "r", encoding="utf-8") as f:
        data = json.load(f)

    doc_id = os.path.splitext(os.path.basename(file_path))[0]

    if "info" in data:
        # 판결문 처리
        doc_text, metadata = process_judgement_data(data)
    elif "clauseField" in data:
        # 약관 처리
        doc_text, metadata = process_clause_data(data)
    else:
        print(f"알 수 없는 데이터 구조: {file_path}")
        return

    if not doc_text.strip():
        print(f"문서 텍스트가 비어 있습니다: {file_path}")
        return

    try:
        # 데이터 디버깅 출력
        print(f"문서 ID: {doc_id}")
        print(f"문서 내용: {doc_text[:100]}...")  # 길면 자르기
        print(f"메타데이터: {metadata}")

        collection.upsert(
            documents=[doc_text],
            metadatas=[metadata],
            ids=[doc_id]
        )
        print(f"성공적으로 저장: {doc_id}")
    except Exception as e:
        print(f"데이터 저장 실패: {doc_id}, 오류: {e}")

# 데이터 디렉토리 확인 및 처리
if not os.path.exists(judgement_dir) or not os.listdir(judgement_dir):
    print(f"판결문 데이터 디렉토리가 비어있거나 존재하지 않습니다: {judgement_dir}")
else:
    for filename in os.listdir(judgement_dir):
        if filename.endswith(".json"):
            process_file(os.path.join(judgement_dir, filename), judgement_collection)

if not os.path.exists(clause_dir) or not os.listdir(clause_dir):
    print(f"약관 데이터 디렉토리가 비어있거나 존재하지 않습니다: {clause_dir}")
else:
    for filename in os.listdir(clause_dir):
        if filename.endswith(".json"):
            process_file(os.path.join(clause_dir, filename), clause_collection)

print("데이터 삽입 완료!")
