# Sutda Example
섯다를 C# 기반으로 구현한 Unity 예제 프로젝트입니다.  
유니티 **Netcode for GameObject** 를 이용하여 멀티플레이 환경을 구성했습니다.

## 프로젝트 구조

```plaintext
SutdaExample/
 ├── CardStruct.cs      # 카드 데이터 구조
 ├── GameManager.cs     # 호스트의 게임 진행 관리
 └── NetGameManager.cs  # 각 클라이언트의 연출 및 rpc 함수 관리
```
## 주요 기능

- **족보 판정 로직**  
  - 광땡, 장땡, 알리, 독사 등 전통 섯다 족보 판정 가능  
  - 족보 우선순위에 따른 승자 계산

- **게임 진행**  
  - 플레이어 수에 따른 패 분배  
  - 라운드별 승자 판정 및 결과 출력
  - 콜, 하프, 올인, 다이 베팅 시스템 구현

---
