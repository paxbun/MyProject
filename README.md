# Bootstrap

```powershell
Import-Module .\bootstrap.psm1;
Set-ProjectName "Project.Name";
```

# Configurations

## Database

* DbConnectionString: MySQL Connection String
* AdminUsername: 관리자 ID
* AdminPassword: 관리자 비밀번호
* AdminRealName: 관리자 실명

## Authentication

* JwtIssuer
* JwtAudience

## CORS

* CorsOrigin: CORS를 허용할 Origin 이름 (맨 끝에 `/`를 달지 말아야 함)

## Azure Blob Storage

* StorageConnectionString: Azure Blob Storage Connection String
* StorageLogTableNamePrefix: 로그를 저장할 Azure Table Storage 내 Table 이름 접두사
* StorageLogQueueCount: 한번에 보낼 로그 개수