# 응소실 5조

## Github 가이드라인 (Window)
1. **기본 세팅**
- Visual Studio 2022 실행 후 시작 창 우측의 **[리포지토리 복제(Clone a repository)]** 클릭
- GitHub 레포지토리 URL(`https://github.com/...`) 복사 후 붙여넣기
- 내 컴퓨터에 저장할 폴더(로컬 경로) 지정 후 **[복제(Clone)]** 클릭
- 복제가 완료되면 우측 솔루션 탐색기에서 `.sln` 파일을 더블클릭하여 프로젝트 열기
2. **소스코드 update**
- 우측 하단 브랜치가 ```main```인지 확인 후 현재 개발 상황 update: 상단 메뉴 [Git] ➔ [끌어오기(Pull)]

  ```bash
  git pull origin main
  ```
3. **branch 생성**
- **VS 2022:** 상단 메뉴 **[Git]** ➔ **[새 브랜치(New Branch)]**
- **이름 규칙:** `작업종류/기능이름` (예: `feature/login`, `bugfix/ui-error`)
  
  ```bash
  git switch -c 브랜치 이름
  ```
- 생성 후 해당 branch로 자동 이동(Checkout) 되었는지 우측 하단 상태표시줄에서 확인
- branch 등록 후 새로 만든 branch에서 코드 작성 
  ```bash
  git push -u origin 브랜치 이름
  ```
  
4. **commit & pull**
- **VS 2022:** 우측 **[Git 변경 내용]** 탭 열기
- 메시지 입력 칸에 작업 설명 작성 (예: "로그인 UI 디자인 및 인증 로직 추가")
- **[모두 커밋(Commit All)]** 버튼 옆의 화살표(▼)를 누르고 **[모두 커밋 후 푸시(Commit All and Push)]** 클릭
5. **Branch Merge**
- GitHub 웹사이트 접속 ➔ 프로젝트 레포지토리로 이동
- 상단에 뜬 노란색 **[Compare & pull request]** 버튼 클릭
- 어떤 기능을 만들었는지 간단히 설명을 적고 **[Create pull request]** 클릭
