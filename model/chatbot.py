import os
import json
from xml.dom.minidom import Document
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

# 판결문 컬렉션 생성
judgement_collection = client.get_or_create_collection(
    name="judgement_docs",
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

def process_file(file_path: str, collection):
    with open(file_path, "r", encoding="utf-8") as f:
        data = json.load(f)

    doc_id = os.path.splitext(os.path.basename(file_path))[0]

    # 판결문만 처리
    if "info" in data:
        doc_text, metadata = process_judgement_data(data)
    else:
        print(f"알 수 없는 데이터 구조(판결문 아님): {file_path}")
        return

    if not doc_text.strip():
        print(f"문서 텍스트가 비어 있습니다: {file_path}")
        return

    try:
        print(f"문서 ID: {doc_id}")
        print(f"문서 내용(일부): {doc_text[:100]}...")
        print(f"메타데이터: {metadata}")

        # documents, embeddings, metadatas, ids 순서에 맞춰 호출해야 할 수 있음
        # ChromaDB 버전에 따라 인자 순서/방식 확인 필요
        # 아래는 예시이며, 실제로는 문서화된 시그니처에 맞게 호출하세요.
        collection.add([doc_text], None, [metadata], [doc_id])


        print(f"성공적으로 저장: {doc_id}")
    except Exception as e:
        print(f"데이터 저장 실패: {doc_id}, 오류: {e}")

# 판결문 데이터 삽입
if not os.path.exists(judgement_dir) or not os.listdir(judgement_dir):
    print(f"판결문 데이터 디렉토리가 비어있거나 존재하지 않습니다: {judgement_dir}")
else:
    for filename in os.listdir(judgement_dir):
        if filename.endswith(".json"):
            process_file(os.path.join(judgement_dir, filename), judgement_collection)

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

def generation_model(context_text, question):
    # 유니코드 정규화나 ASCII 변환 로직이 필요하다면 여기서 추가
    system_prompt = (
        "아래는 판결문에서 발췌한 내용입니다. 사용자의 법률 관련 질문에 대해, "
        "아래 판결문 내용을 참조하여 도움이 되는 답변을 제공하세요. "
        "단, 변호사가 아닌 단순 참고용 조언 형태로 답변하고, 법적 효력에 대한 단정은 피해주세요.\n\n"
        f"참고 내용:\n{context_text}"
    )

    completion = openai.ChatCompletion.create(
        model='gpt-4o-mini',  # 실제 사용 가능한 모델명으로 변경
        messages=[
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": question}
        ],
        temperature=0.7
    )
    answer = completion.choices[0].message.content.strip()
    return answer

def main():
    print("\n판결문 데이터 확인:")
    check_collection_data(judgement_collection)

    print("데이터 검색 시스템에 오신 것을 환영합니다.")
    while True:
        query = input("\n검색어를 입력하세요 (종료하려면 엔터): ").strip()
        if not query:
            # print("검색 시스템을 종료합니다.")
            break

        print("판결문에서 검색 중...")
        results = search_in_collection(judgement_collection, query)
        if not results:
            # print("검색 결과가 없습니다.")
            results = "관련 정보 없음"
            answer = generation_model(results, query)
            print(answer)
        else:
            # print("\n검색 결과:")
            for result in results:
                print("결과나옴")
                # print(f"\nID: {result['id']}")
                # print(f"Score: {result['score']}")
                # print(f"Document: {result['document']}")
                # print(f"Metadata: {result['metadata']}")

            answer = generation_model(results, query)
            print(answer)

if __name__ == "__main__":
    main()
