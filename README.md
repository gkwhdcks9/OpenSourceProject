# 📌 LawBlocks

> **프로젝트 개요**  
> **LawBlocks** 는 메타버스 환경을 활용하여 **법률 지식**에 쉽게 접근하고, 게임적 재미 요소를 통해 **법무사와의 상담 경험**을 혁신하는 프로젝트입니다.  
> 몰입감 있는 가상 공간에서 법률 상식 O/X 퀴즈와 문제 해결을 돕는 법무사 챗봇을 만나보세요!

---

## 🛠 주요 기능

### 🤖 AI Scene
- **법무사 Chatbot**: 상황을 AI에게 알려주면 함께 해결책을 모색할 수 있습니다.

### 📚 Archive Scene
- **게시판(Board)**: 법적 이슈나 궁금한 점을 커뮤니티에 공유
- **기록실(Record Room)**: 과거 상담 내역 조회 및 관리

### ❌✅ O/X Scene
- **O/X 게임**: 법률 상식 퀴즈를 통해 지식 점검, 정답 시 Chatbot 사용 토큰 획득

---

## 🛠 전체 시스템 구조

![image](https://github.com/user-attachments/assets/8aec1d9c-12e7-49d4-b93a-41268e4a8237)

---

## 👥 팀 소개

| 이름    | 역할                                   | GitHub                                              |
|---------|----------------------------------------|-----------------------------------------------------|
| 하종찬  | 팀장 & AI 씬 개발                      | [gkwhdcks9](https://github.com/gkwhdcks9)            |
| 김원호  | 로그인 씬, Archive 씬, DB 관리          | [wonotter](https://github.com/wonotter)             |                                       
| 박선화  | O/X 씬, AI 통신, 데이터 정제            | [paksh0725](https://github.com/paksh0725)           |
| 최승민  | 전체 씬 관리 & 통합                     | [c3110y3110](https://github.com/c3110y3110)         |

---

## 보완할 점
1. **O/X 씬 당위성**: 게임을 통한 화폐 지급 구조 명확화 필요  
2. **AI 모델 변경 고려**: GPT-4에서 API만 활용하는 방향 검토  
3. **통신 보안 강화**: Local 서버 환경 구축 등 추가적인 보안 조치 필요

---

## 유사한 어플 조사
- **디케이엘파트너스 법률사무소**: 세미나 위주의 서비스라 본 프로젝트와 지향점 다름  
- **엘박스(L-Box)**: 전문성 위주로, 대중적 법의식 개선 및 접근성 측면과는 차이가 있음  
=> 현재 기획 방향에서 크게 신경쓰지 않아도 될 것으로 판단

---

## 📅 프로젝트 진행 일정 (Timeline)

![image](https://github.com/user-attachments/assets/364bf220-ad71-435b-a647-a51040e4d6f7)

---
## 📅 프로젝트 개발일지
### 10월  
- **10/9**
  - 아이디어 도출 회의(1차) → 아이디어 결정 회의까지 아이디어 하나씩 가져오기[김원호, 박선화, 최승민, 하종찬]
- **10/16**
  - 아이디어 결정 회의(2차) → '법무사 챗봇' 결정[김원호, 박선화, 최승민, 하종찬] 
- **10/27**
  - Unity AI 씬 디자인 시작[하종찬]
- **10/28**
  - 데이터 분석 시작[박선화]  
- **10/31**
  - Unity AI 씬 디자인 에셋 적용 (Top-Down 2D RPG Assets Pack)[하종찬]

### 11월  
- **11/03**
  - 데이터 분석 완료[박선화]
- **11/04**
  - Spring 통신 개발 시작[김원호]
- **11/05**
  - 개발현황 공유 및 3차 회의[김원호, 박선화, 최승민, 하종찬] → AI 씬 기능 추가(대화내역 정지, 초기화), 기록실/게시판 도메인별로
- **11/06**
  - Unity AI 씬 디자인 완료[하종찬]  
- **11/07**
  - Unity AI 씬 기능 시작[하종찬]  
- **11/08**
  - Home 씬 디자인 시작 (Modern exteriors - RPG Tileset), 회원가입/로그인 시작[최승민]
- **11/10**
  - Unity AI 씬 말풍선 크기 조정 문의[하종찬]
  - Spring 통신 개발 완료[김원호]  
- **11/14** 주요 논제 토의(4차)[김원호, 박선화, 최승민, 하종찬]
  - 로그인/회원가입 별도 Scene 구성  
  - DB: Spring 사용[김원호]  
  - 챗봇-법정 순서 변경  
  - 홈 화면 게임풍 구성(NPC와 상호작용)  
  - 법정 사건 저장 방안(박물관 Object 활용)  
  - O/X 게임 화폐 지급  
  - 법정 오브젝트 구성도 논의
- **11/16**
  - Home 씬 디자인 완료[최승민]
- **11/18**
  - O/X 씬 디자인 시작[박선화]  
- **11/21**
  - O/X 씬 기능 연동 시작[박선화]  
- **11/22**
  - 회원가입/로그인 완료[김원호]
  - O/X 씬 디자인 완료[박선화]
  - 기록실·게시판 기능 연동 시작[김원호]
- **11/23**
  - Archive 씬 디자인 시작[김원호]
  - DB 통합 관리 시작[박선화]
- **11/25**
  - Flask 통신 개발(AI) 시작[박선화]
  - Multi Agent LLM 시작[박선화]  
- **11/27**
  - Archive 씬 디자인 완료[김원호]
  - 기록실 기능 연동 완료[김원호]
- **11/28**
  - 애니메이션 시작[하종찬]
  - 튜토리얼 시작[하종찬]
  - LLM 통합 시작[박선화]
  - 게시판 도메인별 DB 논의
    -> 게시판 도메인별로 관리하는지?, 그러면 도메인별로 각 DB를 생성해야하나?[김원호]
    ->도메인별로 각 DB를 만들어서 관리해야함[최승민]

- **11/29**
  - DB 통합 관리 완료[박선화]
- **11/30**
  - 애니메이션 완료[하종찬]
- **11/31**
  - 개발일정 공유 및 5차 회의[김원호, 박선화, 최승민, 하종찬]
  - 튜토리얼 중단[하종찬] -> AI 씬 디버그
  - KG 구성 삭제[박선화]

### 12월  
- **12/01**
  - 튜토리얼 중단(디버깅 우선)[하종찬]
  - O/X 씬 기능 연동 완료[[박선화]
  - Flask 통신 개발(AI) 완료[박선화]
- **12/03**
  - 전체 통합과정 시작[최승민]  
- **12/06**
  - 게시판 기능 연동 완료[김원호]
- **12/09**
  - Multi Agent LLM 완료[박선화]  
- **12/10**
  - LLM 전체 통합 개발 완료[박선화]
  - 전체 통합과정 완료[최승민]
- **12/11**
  - 최종 회의[김원호, 박선화, 최승민, 하종찬]

---

## ⚙️ 기술 스택 & 아키텍처 (Tech Stack & Architecture)

- **프론트엔드**: Unity, C#  
- **백엔드**: Spring Framework, Flask (AI 통신)  
- **DB**: MySQL or Firebase  
- **AI/ML**: Chatbot API

---

## 🎥 데모 & 스크린샷 (Demo & Screenshots)

- **시작 화면**  
  ![시작 화면 이미지](https://github.com/user-attachments/assets/2dbf8320-a75c-4287-b859-9f6e2f00bd58)

- **회원가입 화면**  
  ![회원가입 화면 이미지](https://github.com/user-attachments/assets/92565e3b-2481-46b2-968f-4989f55c5113)

- **회원가입 인증번호 발송 메일**  
  ![회원가입 인증번호 발송 메일 이미지](https://github.com/user-attachments/assets/824c4f0f-2a2c-4351-b7d1-e7e6723a017f)

- **로그인 화면**  
  ![로그인 화면 이미지](https://github.com/user-attachments/assets/459eb7eb-0232-4084-b83a-839fa4668293)

- **아이디 찾기 화면**  
  ![아이디 찾기 화면 이미지](https://github.com/user-attachments/assets/5587985b-908a-44fc-a44d-84f8fcefd66c)

- **웰컴 화면**  
  ![웰컴 화면 이미지](https://github.com/user-attachments/assets/037a3686-9a8e-49b6-be76-23755804963c)

- **홈 화면**  
  ![홈 화면 이미지](https://github.com/user-attachments/assets/aeeac0c3-dc55-4532-a411-d5f68ce78b9a)

- **O/X 게임 화면**  
  ![O/X 게임 화면 이미지](https://github.com/user-attachments/assets/88d2fe82-338b-4b7f-bc1a-32965f289162)

- **O/X 게임 종료 화면**  
  ![O/X 게임 종료 화면 이미지](https://github.com/user-attachments/assets/4c039ffb-da39-4f91-b1d9-7f28457e098e)

- **챗봇 화면**  
  ![챗봇 화면 이미지](https://github.com/user-attachments/assets/cd740366-e8cf-4c08-95a7-00234caf7147)

- **아카이브 화면**  
  ![아카이브 화면 이미지](https://github.com/user-attachments/assets/04f8ade2-3a76-46cf-bedf-30e1ac5b36a0)

- **게시판 화면**  
  ![게시판 화면 이미지](https://github.com/user-attachments/assets/3a695099-23b4-43b1-bbce-98b16d7c7400)

- **게시판 상세보기 화면**  
  ![게시판 상세보기 화면 이미지](https://github.com/user-attachments/assets/15c540d7-69a3-4a2a-9f18-b6a55d5f94ef)



---

## 📝 참조한 오픈소스 & 에셋

아래는 프로젝트 구현에 참고한 오픈소스, 에셋, 폰트, UI 요소 등을 모은 섹션입니다.

### 유니티 기반 개발
- QA 프리팹 생성  
  ![QA Prefab 1](https://github.com/user-attachments/assets/697b9af5-7b2b-46dc-95a9-4bbb5ca50c2f)  
  ![QA Prefab 2](https://github.com/user-attachments/assets/670d7555-dbc8-4c47-8a57-854b0d3acd38)

- 도메인별 Canvas  
  ![Canvas](https://github.com/user-attachments/assets/1dd511d6-5a87-4b7f-babc-3d13833fd37d)

### 에셋 활용
- 씬 제작  
  ![Scene 1](https://github.com/user-attachments/assets/d93d81d9-ff63-4bb3-816b-6e8858f3bca6)  
  ![Scene 2](https://github.com/user-attachments/assets/e24f4c39-9e93-4410-8d18-c7fa6fda6dac)  
  ![Scene 3](https://github.com/user-attachments/assets/f42ce5f4-b414-4bc4-a5e1-db6bea0a7850)

- 챗봇 씬 제작  
  ![Chatbot Scene](https://github.com/user-attachments/assets/d1cfe801-629e-4a77-a548-0bfb410c27ea)

### 폰트 & 버튼
- Font  
  ![Font](https://github.com/user-attachments/assets/377db13d-3923-42e3-a0ed-f4c0c0b139b4)
  
- Button  
  ![Button 1](https://github.com/user-attachments/assets/f714d136-b3ac-4267-b395-42b54304c04f)  
  ![Button 2](https://github.com/user-attachments/assets/acd97558-5477-4115-a64f-68c046829005)

### 판결문 데이터 구축
  ![판결문 1](https://github.com/user-attachments/assets/e762fc88-626c-4229-82b4-8d1f1adc8f54)  
  ![판결문 2](https://github.com/user-attachments/assets/dca464cf-fed4-4f64-b149-beddeee92ae2)

### ChromaDB 활용
  ![ChromaDB 1](https://github.com/user-attachments/assets/6f86e298-f4db-4d2b-8fca-741fcbdaab29)  
  ![ChromaDB 2](https://github.com/user-attachments/assets/f6a3d108-aaff-4467-86a0-fc597b15b9b1)

### 통신 관련
  ![통신 1](https://github.com/user-attachments/assets/3b8e46d3-60fa-4155-927a-8d5cf5462ee7)  
  ![통신 2](https://github.com/user-attachments/assets/3a1f9b2a-6b4f-4e0d-8bd4-da1b90cc1f12)  
  ![통신 3](https://github.com/user-attachments/assets/1c3f5e11-efd7-4234-904e-e8806ac57915)  
  ![통신 4](https://github.com/user-attachments/assets/fe8a50a9-8041-4afa-9a47-18c86a83fce7)

---

**LawBlocks**는 앞으로도 더 나은 법률 서비스 경험을 위해 꾸준히 개선할 예정입니다.  
관심과 피드백은 언제든지 환영합니다! 🙌
