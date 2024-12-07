import os
import json
import openai
import chromadb
from chromadb.utils import embedding_functions
from typing import List
from flask import Flask, request, jsonify
from flask_cors import CORS

# 환경 변수 설정
openai.api_key = os.getenv("OPENAI_API_KEY")
if not openai.api_key:
    raise EnvironmentError("OPENAI_API_KEY 환경 변수가 설정되지 않았습니다.")

os.environ["CHROMA_DB_IMPL"] = "duckdb+parquet"
os.environ["CHROMA_PERSIST_DIRECTORY"] = "./chroma_db"

# 임베딩 함수 정의
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

# ChromaDB 초기화
client = chromadb.Client()
embedding_fn = OpenAIEmbeddingFunction()

judgement_dir = "./sample/판결문/라벨링데이터"

# 컬렉션 가져오기/생성
judgement_collection = client.get_or_create_collection(
    name="judgement_docs",
    # embedding_function=embedding_fn # 필요 시 활성화
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

        collection.add([doc_text], None, [metadata], [doc_id])
        print(f"성공적으로 저장: {doc_id}")
    except Exception as e:
        print(f"데이터 저장 실패: {doc_id}, 오류: {e}")

def check_collection_data(collection):
    data = collection.get()
    info = {
        "count": len(data["ids"]),
        "documents": []
    }
    for i, doc in enumerate(data['documents']):
        info["documents"].append({
            "id": data['ids'][i],
            "document": doc,
            "metadata": data['metadatas'][i]
        })
    return info

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

def generation_model(conversation_history, context_text, question):
    messages = []
    for message in conversation_history:
        role = message.get("role")
        content = message.get("content")
        if role and content:
            messages.append({"role": role, "content": content})

    system_prompt = (
        "아래는 판결문에서 발췌한 내용입니다. 사용자의 법률 관련 질문에 대해, "
        "아래 판결문 내용을 참조하여 도움이 되는 답변을 제공하세요. "
        "단, 변호사가 아닌 단순 참고용 조언 형태로 답변하고, 법적 효력에 대한 단정은 피해주세요.\n\n"
        f"참고 내용:\n{context_text}"
    )

    messages.insert(0, {"role": "system", "content": system_prompt})
    messages.append({"role": "user", "content": question})

    try:
        completion = openai.ChatCompletion.create(
            model='gpt-4',  # 실제 사용 가능한 모델명으로 변경 필요
            messages=messages,
            temperature=0.7
        )
        answer = completion.choices[0].message.content.strip()
        return answer
    except Exception as e:
        print(f"OpenAI API 호출 중 오류 발생: {e}")
        return "죄송합니다. 답변을 생성하는 중에 오류가 발생했습니다."



################################
# Flask API 서버 설정
################################
app = Flask(__name__)
CORS(app)

# 판결문 인덱싱 API
@app.route('/index_documents', methods=['POST'])
def index_documents():
    try:
        if not os.path.exists(judgement_dir) or not os.listdir(judgement_dir):
            return jsonify({"error": f"판결문 데이터 디렉토리가 비어있거나 존재하지 않습니다: {judgement_dir}"}), 400
        for filename in os.listdir(judgement_dir):
            if filename.endswith(".json"):
                process_file(os.path.join(judgement_dir, filename), judgement_collection)
        return jsonify({"message": "데이터 삽입 완료"}), 200
    except Exception as e:
        return jsonify({"error": str(e)}), 500

# 컬렉션 데이터 확인 API
@app.route('/check_collection_data', methods=['GET'])
def api_check_collection_data():
    try:
        info = check_collection_data(judgement_collection)
        return jsonify(info), 200
    except Exception as e:
        return jsonify({"error": str(e)}), 500

# 검색 API
@app.route('/search', methods=['POST'])
def api_search():
    try:
        data = request.get_json()
        query = data.get("query", "")
        if not query:
            return jsonify({"error": "query 파라미터가 없습니다."}), 400

        results = search_in_collection(judgement_collection, query)
        return jsonify({"results": results}), 200
    except Exception as e:
        return jsonify({"error": str(e)}), 500

# GPT 답변 생성 API
@app.route('/ask', methods=['POST'])
def ask():
    try:
        data = request.get_json()
        query = data.get("query", "")
        conversation_history = data.get("conversation_history", [])  # List of messages

        if not isinstance(conversation_history, list):
            return jsonify({"error": "conversation_history는 리스트 형태여야 합니다."}), 400

        # 관련 문서 검색
        results = search_in_collection(judgement_collection, query)
        if not results:
            results_context = "관련 정보 없음"
        else:
            retrieved_docs = [r["document"] for r in results]
            results_context = "\n\n".join(retrieved_docs)

        # OpenAI 모델을 사용한 답변 생성
        answer = generation_model(conversation_history, results_context, query)
        return jsonify({"answer": answer}), 200
    except Exception as e:
        return jsonify({"error": str(e)}), 500


# Unity로부터 데이터 수신 예제 API
@app.route('/receive_data', methods=['POST'])
def receive_data():
    try:
        data = request.get_json()
        category = data.get("category", 0.0)
        message = data.get("message", "")

        print(f"Received Data - Category: {category}, Message: {message}")

        response_message = f"{message}"
        response = {
            "category": category,
            "message": response_message
        }
        print(response)
        return jsonify(response), 200

    except Exception as e:
        print(f"Error: {e}")
        return jsonify({"error": "Something went wrong"}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
