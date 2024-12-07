import os
import openai
import chromadb
from chromadb.utils import embedding_functions
from typing import List

# OpenAI API Key를 환경 변수에서 가져오기
openai.api_key = os.getenv("OPENAI_API_KEY")

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

# 임베딩 함수 인스턴스 생성
embedding_fn = OpenAIEmbeddingFunction()

# 판결문 및 약관 컬렉션 가져오기 (컬렉션이 없으면 생성)
judgement_collection = client.get_or_create_collection(
    name="judgement_docs",
    embedding_function=embedding_fn
)

clause_collection = client.get_or_create_collection(
    name="clause_docs",
    embedding_function=embedding_fn
)

def check_collection_data(collection):
    # 컬렉션에서 모든 데이터를 가져오기
    data = collection.get()
    print(f"컬렉션에 저장된 데이터 개수: {len(data['ids'])}")
    for i, doc in enumerate(data['documents']):
        print(f"\n--- Document {i+1} ---")
        print(f"ID: {data['ids'][i]}")
        print(f"Document: {doc}")
        print(f"Metadata: {data['metadatas'][i]}")

def search_in_collection(collection, query: str, n_results: int = 5):
    """
    특정 컬렉션에서 쿼리와 유사한 문서를 검색하는 함수
    """
    try:
        # ChromaDB의 query 메서드 호출
        results = collection.query(
            query_texts=[query],  # 검색할 텍스트
            n_results=n_results   # 반환할 결과 수
        )

        # 결과 정리
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

    # 약관 데이터 확인
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

        # 사용자가 선택한 데이터 유형에 따라 컬렉션 설정
        if choice == "1":
            collection = judgement_collection
            print("판결문에서 검색 중...")
        else:  # choice == "2"
            collection = clause_collection
            print("약관에서 검색 중...")

        # 검색 실행
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
