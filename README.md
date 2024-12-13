# 📌 LawBlocks

> **프로젝트 개요**  
> **LawBlocks** 는 메타버스 환경을 활용하여 **법률 지식**에 쉽게 접근하고, 게임적 재미 요소를 통해 **법무사와의 상담 경험**을 혁신하는 프로젝트입니다.  
> 몰입감 있는 가상 공간에서 법률 상식 O/X 퀴즈와 문제 해결을 돕는 법무사 챗봇을 만나보세요!

---

## 🛠 주요 기능

### 🤖 AI Scene
- **법무사 Chatbot**: 당신의 상황을 AI에게 알려주면, 함께 해결책을 모색합니다.  
  법률적 궁금증을 해소하고, 조언을 얻어보세요!

### 📚 Archive Scene
- **게시판(Board)**: 법적 이슈나 궁금한 점을 커뮤니티에 공유할 수 있습니다.  
- **기록실(Record Room)**: 과거 질문했던 상담 내역을 조회하고, 정리된 정보를 다시 확인할 수 있습니다.

### ❌✅ O/X Scene
- **O/X 게임**: 법률 상식 퀴즈를 통해 지식을 점검하고, 정답 시 Chatbot에 사용할 토큰을 획득하세요!

---

## 👥 팀 소개

| 이름    | 역할                                  | GitHub                                              |
|---------|---------------------------------------|-----------------------------------------------------|
| 하종찬  | 팀장 & AI 씬 개발                     | [gkwhdcks9](https://github.com/gkwhdcks9)          |
| 김원호  | 로그인 씬 & Archive 씬 & DB 관리       |                                                     |
| 박선화  | O/X 씬 & AI 통신 & 데이터 정제         |                                                     |
| 최승민  | 전체 씬 관리 & 통합                    |                                                     |

> 다른 팀원 정보는 추후 업데이트 가능합니다.

---
## 보완할 점

1. O/X 씬 당위성
-> 화폐를 지급
2. AI 모델 변경?
-> GPT - 4o에서 API만
3. 통신 보안
-> local 서버

---

## 유사한 어플
-디케이엘파트너스 법률사무소 -> 목적이 너무 세미나에 초점이 맞혀짐
-엘박스 -> 너무 전문적이라 우리가 목표로 잡은 법의식 개선이나 접근성 측면에서 방향이 다름
=> 그렇게 신경쓰지 않아도 될 것 같다.

---

## 📅 프로젝트 진행 일정

아래는 아이디어 결정부터 전체 통합 완료까지의 타임라인입니다.  
프로젝트의 흐름과 발전 과정을 한눈에 살펴보세요!

### 10월  
- **10/26** 아이디어 결정 회의 → '법무사 챗봇' 결정  
- **10/27** Unity AI 씬 디자인 시작  
- **10/28** 데이터 분석 시작  
- **10/31** Unity AI 씬 디자인 에셋 적용 (Top-Down 2D RPG Assets Pack)

### 11월  
- **11/03** 데이터 분석 완료  
- **11/04** Spring 통신 개발 시작  
- **11/06** Unity AI 씬 디자인 완료  
- **11/07** Unity AI 씬 기능 시작  
- **11/08** Home 씬 디자인 시작 (Modern exteriors - RPG Tileset), 회원가입/로그인 시작  
- **11/10** Unity AI 씬 말풍선 크기 조정 문의, Spring 통신 개발 완료  
- **11/14** 주요 논제 토의 (씬 구성, DB 관리, 토큰 처리 등)  
- **11/16** Home 씬 디자인 완료  
- **11/18** O/X 씬 디자인 시작  
- **11/21** O/X 씬 기능 연동 시작
- **11/21** 주요 안건 회의
1. 로그인씬 구성 
2. 회원가입씬 구성
   -> 로그인/회원가입은 따로 Scene을 만들어서
3. DB 관리
   -> Spring으로 구현[김원호]
4. 기존 [챗봇 → 법정] 순서에서 [법정 → 챗봇] 순으로 변경
5. 홈 화면 구성 (게임같이, 다른 씬들과 동작법이 같도록..)
   -> 소울 나이트같은 게임처럼 홈화면에서 조작해서 기능담당 NPC와 상호작용
6. 법정에서 사건들을 어떻게 저장할 것인가
   -> 박물관에서 Object를 이용해 상호작
7. O/X 게임의 동기 설정
   -> 화폐를 준다
8. 법정 개별 오브젝트 구성과 조작법 전체 구성도

- **11/22** 회원가입/로그인 완료, O/X 씬 디자인 완료, 기록실·게시판 기능 연동 시작  
- **11/23** Archive 씬 디자인 시작, DB 통합 관리 시작  
- **11/25** Flask 통신 개발(AI) 시작, Multi Agent LLM 시작  
- **11/27** Archive 씬 디자인 완료, 기록실 기능 연동 완료  
- **11/28** 애니메이션 시작, 튜토리얼 시작, LLM 전체 통합 개발 시작, 게시판 도메인별 DB 논의  
- **11/29** DB 통합 관리 완료  
- **11/30** 애니메이션 완료

### 12월  
- **12/01** 튜토리얼 중단(디버깅 우선), O/X 씬 기능 연동 완료, Flask 통신 개발(AI) 완료  
- **12/03** 전체 통합과정 시작  
- **12/06** 게시판 기능 연동 완료  
- **12/09** Multi Agent LLM 완료  
- **12/10** LLM 전체 통합 개발 완료, 전체 통합과정 완료

---

## ⚙️ 기술 스택 & 아키텍처 (Tech Stack & Architecture)

- **프론트엔드**: Unity, C#  
- **백엔드**: Spring Framework, Flask (AI 통신)  
- **DB**: MySQL or Firebase  
- **AI/ML**: LLM (Multi Agent), Chatbot API

---

## 🎥 데모 & 스크린샷 (Demo & Screenshots)

프로젝트 동작 영상이나 스크린샷을 추가하여 실제 사용 환경을 직관적으로 보여주세요.  
(*추후 GIF, 이미지, 동영상 링크 삽입 예정*)

---

## 📝 참조한 오픈소스

- **유니티**
- QA 프리팹 생성
  -![image](https://github.com/user-attachments/assets/697b9af5-7b2b-46dc-95a9-4bbb5ca50c2f)
  -![image](https://github.com/user-attachments/assets/670d7555-dbc8-4c47-8a57-854b0d3acd38)

- 도메인에 맞는 Canvas
  -![image](https://github.com/user-attachments/assets/1dd511d6-5a87-4b7f-babc-3d13833fd37d)

- **에셋**
- 씬 제작
  -![image](https://github.com/user-attachments/assets/d93d81d9-ff63-4bb3-816b-6e8858f3bca6)
  -![image](https://github.com/user-attachments/assets/e24f4c39-9e93-4410-8d18-c7fa6fda6dac)
  -![image](https://github.com/user-attachments/assets/f42ce5f4-b414-4bc4-a5e1-db6bea0a7850)
  
- 챗봇 씬 제작
  - ![image](https://github.com/user-attachments/assets/d1cfe801-629e-4a77-a548-0bfb410c27ea)

- Font
  -![image](https://github.com/user-attachments/assets/377db13d-3923-42e3-a0ed-f4c0c0b139b4)

- Button
  -![image](https://github.com/user-attachments/assets/f714d136-b3ac-4267-b395-42b54304c04f)
  -![image](https://github.com/user-attachments/assets/acd97558-5477-4115-a64f-68c046829005)

- **판결문 데이터 구축**
  -![image](https://github.com/user-attachments/assets/e762fc88-626c-4229-82b4-8d1f1adc8f54)
  -![image](https://github.com/user-attachments/assets/dca464cf-fed4-4f64-b149-beddeee92ae2)

- **ChromaDB**
  -![image](https://github.com/user-attachments/assets/6f86e298-f4db-4d2b-8fca-741fcbdaab29)
  -![image](https://github.com/user-attachments/assets/f6a3d108-aaff-4467-86a0-fc597b15b9b1)

- **통신**
  -![image](https://github.com/user-attachments/assets/3b8e46d3-60fa-4155-927a-8d5cf5462ee7)
  -![image](https://github.com/user-attachments/assets/3a1f9b2a-6b4f-4e0d-8bd4-da1b90cc1f12)
  -![image](https://github.com/user-attachments/assets/1c3f5e11-efd7-4234-904e-e8806ac57915)
  -![image](https://github.com/user-attachments/assets/fe8a50a9-8041-4afa-9a47-18c86a83fce7)

---

**LawBlocks**는 앞으로도 더 나은 법률 서비스 경험을 위해 기능 향상과 개선을 진행할 예정입니다.  
관심과 피드백은 언제나 환영합니다! 🙌
