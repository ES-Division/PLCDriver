# PLCDriver
ArxPLCBroker PLC Driver Sample Code

## 정의

### Namespace

- DriverInterface

### Classes

|  |  |
| --- | --- |
| CommonDriver | PLC Driver 개발하기 위해 정의된 abstract Class |
| CommonDriver.DriverInfo | PLC Driver 정보에 대한 설명을 위해 정의된 Class |
| CommonDriver.TagInfo | PLC Mapping List 정보 정의된 Class |

### Summary

1. **CommonDriver**

| Attribute Members | Description | Type |
| --- | --- | --- |
| ip | PLC IP | string |
| port | PLC Port | int |
| lstTagInfo | PLC Mapping List 정보 | List<TagInfo> |
| driverInfo | PLC Driver 정보 | DriverInfo |

| Properties Members | Description | Type |
| --- | --- | --- |
| abstract Socket sock | PLC 연결 소켓 정보 [연결 상태, Protocol, etc…] | Socket |

| Method Members | Description |
| --- | --- |
| abstract DriverInfo getDriverInfo() | PLC Driver 정보 출력 함수 |
| abstract bool setTagInfo(string path) | PLC Mapping List 정보 설정 함수 |
| abstract bool connect() | PLC 연결 함수 |
| abstract bool disConnect() | PLC 연결 종료 함수 |
| abstract Tuple<bool, Dictionary<string, Dictionary<string, object>>> readAllData() | Mapping List 모든 태그 정보 읽기 함수 |
| abstract bool reads(Dictionary<string, Dictionary<string, object>> dicData) | 복수 요청 태그에 대한 PLC 메모리 데이터 읽기 함수 |
| abstract bool read(string equipmentCode, string tagName, ref object data) | 단일 요청 태그에 대한 PLC 메모리 데이터 읽기 함수 |
| abstract bool writes(Dictionary<string, Dictionary<string, object>> dicData) | 복수 요청 태그에 대한 PLC 메모리 데이터 쓰기 함수 |
| abstract bool write(string equipmentCode, string tagName, object data) | 단일 요청 태그에 대한 PLC 메모리 데이터 쓰기 함수 |

1. **TagInfo**

| Attribute Members | Description | Type |
| --- | --- | --- |
| equipmentCode | 설비 코드 | string |
| name | PLC 메모리 주소 태그 명칭 | string |
| addr | PLC 메모리 주소 | string |
| bitAddr | PLC 메모리 주소 비트 영역 | string |
| size | PLC 메모리 주소 크기 | int |
| memoryName | PLC 메모리 영역 | string |
| memoryType | PLC 메모리 접근 타입 | string |
| isHex | PLC 메모리 주소 Hex 표현 여부 | bool |
1. **DriverInfo**

| Attribute Members | Description | Type |
| --- | --- | --- |
| version | Driver Version 정보 | string |
| provider | Driver 제공자 정보 | string |
| releaseData | Driver 배포 날짜 정보 | string |
| protocolType | PLC Protocol 정보 | string |
| driverName | Driver 이름 | string |
| manufactureData | 제공자가 원하는 ManufactureData 추가 정보 | string |

### Method Members

1. **getDriverInfo**

|  | public abstract DriverInfo getDriverInfo() |
| --- | --- |
| Description | PLC Driver의 정보를 반환합니다. |
| Parameters | N/A |
| Return Value | Type : DriverInfo , Description : PLC Driver 정보 정의 Class
[실패 시 null 반환] |
1. **setTagInfo**

|  | public abstract bool setTagInfo(string path) |
| --- | --- |
| Description | PLC IP, Port, Mapping List 정보 설정합니다. |
| Parameters | Type : string , Description : 설정 파일 위치 (파일 명 포함) |
| Return Value | Type : bool , Description : PLC Driver 설정 정보 설정 성공 여부 |

<aside>
💡 PLC와 인터페이스를 하기 위해 PLC Driver 생성 후에 setTagInfo 함수를 호출해야 한다.

</aside>

1. **connect**

|  | public abstract bool connect() |
| --- | --- |
| Description | 설정된 IP, Port 정보로 PLC에 연결을 요청합니다. |
| Parameters | N/A |
| Return Value | Type : bool , Description : 연결 성공 여부 |
1. **disConnect**

|  | public abstract bool disConnect() |
| --- | --- |
| Description | 연결된 PLC와 연결 종료를 요청합니다. |
| Parameters | N/A |
| Return Value | Type : bool , Description : 연결 종료 성공 여부 |
1. **readAllData**

|  | public abstract Tuple<bool, Dictionary<string, Dictionary<string, object>>> readAllData() |
| --- | --- |
| Description | 설정된 PLC Mapping List 의 메모리 데이터 전부 읽어 옵니다. |
| Parameters | N/A |
| Return Value | <Tuple>
Type : bool , Description : 데이터 읽기 성공 여부
Type : Dictionary<string, Dictionary<string, object>> : 설정된 모든 PLC 메모리 데이터 집합체
[key : [string]설비 구분 코드, value : <key : [string]메모리 주소 이름, value : [object] PLC 메모리 데이터 - 메모리 주소(key) 매칭 데이터 읽기 실패 시 null(value)>] |
1. **reads**

|  | public abstract bool reads(Dictionary<string, Dictionary<string, object>> dicData) |
| --- | --- |
| Description | 사용자가 요청한 여러 메모리 데이터를 읽어 옵니다. |
| Parameters | Type : Dictionary<string, Dictionary<string, object>> : 요청한  PLC 메모리 데이터 집합체
[key : [string] 설비 구분 코드, value : < key : [string] 메모리 주소 이름, value : [object] PLC 메모리 데이터 - 메모리 주소(key) 매칭 데이터 읽기 실패 시 null(value) >] / |
| Return Value | Type : bool , Description : 데이터 읽기 성공 여부 |
1. **read**

|  | public abstract bool read(string equipmentCode, string tagName, ref object data) |
| --- | --- |
| Description | 사용자가 요청한 하나의 메모리 데이터를 읽어 옵니다. |
| Parameters | Type : string , Decription : 설비 구분 코드
Type : string , Decription : 읽기 요청 메모리 주소 이름
Type : object, Description : 반환 받을 PLC 메모리 데이터 |
| Return Value | Type : bool , Description : 데이터 읽기 성공 여부 |
1. **writes**

|  | public abstract bool writes(Dictionary<string, Dictionary<string, object>> dicData) |
| --- | --- |
| Description | 사용자가 요청한 여러 데이터를 요청한 PLC 메모리 주소에 작성합니다. |
| Parameters | Type : Dictionary<string, Dictionary<string, object>> : 쓰기 요청 PLC 메모리 주소/데이터
[key : [string] 설비 구분 코드, value : <key : [string] 메모리 주소 이름, value : [object] PLC 메모리 데이터 >] |
| Return Value | Type : bool , Description : 데이터 쓰기 성공 여부 |
1. **write**

|  | public abstract bool write(string equipmentCode, string tagName, object data) |
| --- | --- |
| Description | 사용자가 요청한 하나의 데이터를 요청한 PLC 메모리 주소에 작성합니다. |
| Parameters | Type : string , Description : 설비 구분 코드
Type : string , Description : 쓰기 요청 메모리 주소 이름
Type : object , Description : PLC 메모리에 작성할 데이터 |
| Return Value | Type : bool , Description : 데이터 쓰기 성공 여부 |
