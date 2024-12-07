import os
import json
import openai
import chromadb
from chromadb.utils import embedding_functions
from typing import List

# OpenAI API Key 설정
openai.api_key = os.getenv("OPENAI_API_KEY")
if not openai.api_key:
    raise EnvironmentError("OPENAI_API_KEY 환경 변수가 설정되지 않았습니다.")

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
embedding_fn = OpenAIEmbeddingFunction()

judgement_dir = "./sample/판결문/라벨링데이터"
clause_dir = "./sample/약관/라벨링데이터"

# 컬렉션 생성 (최신 버전에 맞추어 embedding_function 제거 가능성도 있으니 문서 확인)
judgement_collection = client.get_or_create_collection(
    name="judgement_docs"
    # embedding_function=embedding_fn
)
clause_collection = client.get_or_create_collection(
    name="clause_docs"
    # embedding_function=embedding_fn
)

def validate_and_prepare_metadata(metadata: dict) -> dict:
    new_metadata = {}
    for k, v in metadata.items():
        if isinstance(v, (list, dict)):
            new_metadata[k] = json.dumps(v, ensure_ascii=False)
        elif isinstance(v, (str, int, float, bool)):
            new_metadata[k] = v
        else:
            new_metadata[k] = str(v)
    return new_metadata

def process_judgement_data(data: dict) -> (str, dict):
    text_parts = data.get("facts", {}).get("bsisFacts", []) + \
                 data.get("dcss", {}).get("courtDcss", []) + \
                 data.get("close", {}).get("cnclsns", [])
    document_text = "\n".join(text_parts)
    metadata = {k: v for k, v in data.items() if k not in ["facts", "dcss", "close"]}
    metadata["mediaType"] = "판결문"
    return document_text, validate_and_prepare_metadata(metadata)

def process_clause_data(data: dict) -> (str, dict):
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
        doc_text, metadata = process_judgement_data(data)
    elif "clauseField" in data:
        doc_text, metadata = process_clause_data(data)
    else:
        print(f"알 수 없는 데이터 구조: {file_path}")
        return

    if not doc_text.strip():
        print(f"문서 텍스트가 비어 있습니다: {file_path}")
        return

    try:
        print(f"문서 ID: {doc_id}")
        print(f"문서 내용(일부): {doc_text[:100]}...")
        print(f"메타데이터: {metadata}")

        # upsert → upsert_documents 로 변경
        collection.add(
            documents=[doc_text],
            metadatas=[metadata],
            ids=[doc_id]
        )

        print(f"성공적으로 저장: {doc_id}")
    except Exception as e:
        print(f"데이터 저장 실패: {doc_id}, 오류: {e}")

# 데이터 삽입
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

def check_collection_data(collection):
    data = collection.get()
    print(f"컬렉션에 저장된 데이터 개수: {len(data['ids'])}")
    for i, doc in enumerate(data['documents']):
        print(f"\n--- Document {i+1} ---")
        print(f"ID: {data['ids'][i]}")
        print(f"Document: {doc}")
        print(f"Metadata: {data['metadatas'][i]}")

def search_in_collection(collection, query: str, n_results: int = 5):
    try:
        # query_texts → texts 로 변경 가능성 있음. 문서 참고. 여기서는 texts 사용
        results = collection.query(
            query_texts=[query],
            n_results=n_results
        )
        search_results = []
        for i, document in enumerate(results["documents"]):
            search_results.append({
                "document": document[0] if document else "N/A",
                "metadata": results["metadatas"][i],
                "id": results["ids"][i],
                "score": results["distances"][i]
            })
        return search_results
    except Exception as e:
        print(f"검색 중 오류가 발생했습니다: {str(e)}")
        return []

def main():
    print("\n판결문 데이터 확인:")
    check_collection_data(judgement_collection)

    print("\n약관 데이터 확인:")
    check_collection_data(clause_collection)
    
    print("데이터 검색 시스템에 오신 것을 환영합니다.")
    while True:
        print("\n검색하려는 데이터 유형을 선택하세요:")
        print("1. 판결문")
        print("2. 약관")
        print("3. 종료")
        
        choice = input("선택 (1/2/3): ").strip()
        if choice == "3":
            print("검색 시스템을 종료합니다.")
            break
        
        if choice not in {"1", "2"}:
            print("유효하지 않은 입력입니다. 다시 시도하세요.")
            continue

        query = input("검색어를 입력하세요: ").strip()
        if not query:
            print("검색어를 입력하지 않았습니다. 다시 시도하세요.")
            continue

        if choice == "1":
            collection = judgement_collection
            print("판결문에서 검색 중...")
        else:
            collection = clause_collection
            print("약관에서 검색 중...")

        results = search_in_collection(collection, query)
        if not results:
            print("검색 결과가 없습니다.")
        else:
            print("\n검색 결과:")
            for result in results:
                print(f"\nID: {result['id']}")
                print(f"Score: {result['score']}")
                print(f"Document: {result['document']}")
                print(f"Metadata: {result['metadata']}")

if __name__ == "__main__":
    main()
