
запрос на получение списка чеков

curl ^"https://mco.nalog.ru/api/v1/receipt^" ^
  -H ^"Accept: application/json, text/plain, */*^" ^
  -H ^"Accept-Language: ru,en;q=0.9^" ^
  -H ^"Authorization: Bearer eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJ7XCJhdXRoVHlwZVwiOlwiU01TXCIsXCJsb2dpblwiOlwiNzk4NTI3NTkyMzJcIixcImlkXCI6MzYyNDE3NCxcImRldmljZUlkXCI6XCJ3YnkxYWRWcVdmWkhCVkZBelVmWDhcIixcInBob25lXCI6XCI3OTg1Mjc1OTIzMlwifSIsImV4cCI6MTc3MDU1ODYzNn0.g5TiMLH8FsttP0fyFNLryfbHreUugv0UjXks5Z53d7_v_eI5MeC3yhUrRPx6dMyQR2wGBFfdy2XnOREbTa697Q^" ^
  -H ^"Connection: keep-alive^" ^
  -H ^"Content-Type: application/json;charset=UTF-8^" ^
  -b ^"_ym_uid=1770554971419543089; _ym_d=1770554971; _ym_isad=2; _ym_visorc=b; _ga=GA1.1.2097553673.1770555013; _ga_9R4V3JQRCG=GS2.1.s1770555012^$o1^$g1^$t1770555939^$j60^$l0^$h0^" ^
  -H ^"Origin: https://mco.nalog.ru^" ^
  -H ^"Referer: https://mco.nalog.ru/^" ^
  -H ^"Sec-Fetch-Dest: empty^" ^
  -H ^"Sec-Fetch-Mode: cors^" ^
  -H ^"Sec-Fetch-Site: same-origin^" ^
  -H ^"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 YaBrowser/25.12.0.0 Safari/537.36^" ^
  -H ^"sec-ch-ua: ^\^"Chromium^\^";v=^\^"142^\^", ^\^"YaBrowser^\^";v=^\^"25.12^\^", ^\^"Not_A Brand^\^";v=^\^"99^\^", ^\^"Yowser^\^";v=^\^"2.5^\^"^" ^
  -H ^"sec-ch-ua-mobile: ?0^" ^
  -H ^"sec-ch-ua-platform: ^\^"Windows^\^"^" ^
  --data-raw ^"^{^\^"limit^\^":10,^\^"offset^\^":0,^\^"dateFrom^\^":null,^\^"dateTo^\^":null,^\^"orderBy^\^":^\^"CREATED_DATE:DESC^\^",^\^"inn^\^":null,^\^"kktOwner^\^":^\^"^\^"^}^"

пример ответа

{
    "receipts": [
        {
            "buyer": "79852759232",
            "buyerType": "1",
            "createdDate": "2026-02-07T17:34:00",
            "fiscalDocumentNumber": "78468",
            "fiscalDriveNumber": "7380440902697876",
            "totalSum": "3.90",
            "kktOwner": "ООО \"Т-МОБ\"",
            "kktOwnerInn": "7743200179",
            "key": "79852759232|1|2026-02-07 17:34:00|78468|7380440902697876",
            "brandId": 10898,
            "receiveDate": "2026-02-07T17:34:06"
        },
        {
            "buyer": "79852759232",
            "buyerType": "1",
            "createdDate": "2026-02-07T17:33:00",
            "fiscalDocumentNumber": "88237",
            "fiscalDriveNumber": "7380440902697818",
            "totalSum": "3.90",
            "kktOwner": "ООО \"Т-МОБ\"",
            "kktOwnerInn": "7743200179",
            "key": "79852759232|1|2026-02-07 17:33:00|88237|7380440902697818",
            "brandId": 10898,
            "receiveDate": "2026-02-07T17:33:12"
        },
        {
            "buyer": "79852759232",
            "buyerType": "1",
            "createdDate": "2026-02-07T17:32:00",
            "fiscalDocumentNumber": "78308",
            "fiscalDriveNumber": "7380440902697831",
            "totalSum": "3.90",
            "kktOwner": "ООО \"Т-МОБ\"",
            "kktOwnerInn": "7743200179",
            "key": "79852759232|1|2026-02-07 17:32:00|78308|7380440902697831",
            "brandId": 10898,
            "receiveDate": "2026-02-07T17:32:53"
        },
        {
            "buyer": "79852759232",
            "buyerType": "1",
            "createdDate": "2026-02-01T22:17:00",
            "fiscalDocumentNumber": "49823",
            "fiscalDriveNumber": "7380440902697319",
            "totalSum": "390.00",
            "kktOwner": "ООО \"Т-МОБ\"",
            "kktOwnerInn": "7743200179",
            "key": "79852759232|1|2026-02-01 22:17:00|49823|7380440902697319",
            "brandId": 10898,
            "receiveDate": "2026-02-01T22:18:02"
        },
        {
            "buyer": "79852759232",
            "buyerType": "1",
            "createdDate": "2026-01-31T10:21:00",
            "fiscalDocumentNumber": "27361",
            "fiscalDriveNumber": "7380440903140279",
            "totalSum": "6713.00",
            "kktOwner": "ООО \"ЯНДЕКС.ТАКСИ\"",
            "kktOwnerInn": "7704340310",
            "key": "79852759232|1|2026-01-31 10:21:00|27361|7380440903140279",
            "brandId": 10882,
            "receiveDate": "2026-01-31T10:21:23"
        },
        {
            "buyer": "abakumovoleg@gmail.com",
            "buyerType": "0",
            "createdDate": "2026-01-30T21:07:00",
            "fiscalDocumentNumber": "34726",
            "fiscalDriveNumber": "7380440902673445",
            "totalSum": "988.00",
            "kktOwner": "АО \"ВКУСВИЛЛ\"",
            "kktOwnerInn": "7734443270",
            "key": "abakumovoleg@gmail.com|0|2026-01-30 21:07:00|34726|7380440902673445",
            "brandId": 10339,
            "receiveDate": "2026-01-30T21:07:17"
        },
        {
            "buyer": "abakumovoleg@gmail.com",
            "buyerType": "0",
            "createdDate": "2026-01-30T20:15:00",
            "fiscalDocumentNumber": "136604",
            "fiscalDriveNumber": "7380440902266196",
            "totalSum": "988.00",
            "kktOwner": "АО \"ВКУСВИЛЛ\"",
            "kktOwnerInn": "7734443270",
            "key": "abakumovoleg@gmail.com|0|2026-01-30 20:15:00|136604|7380440902266196",
            "brandId": 10339,
            "receiveDate": "2026-01-30T20:17:08"
        },
        {
            "buyer": "abakumovoleg@gmail.com",
            "buyerType": "0",
            "createdDate": "2026-01-30T18:57:00",
            "fiscalDocumentNumber": "21273",
            "fiscalDriveNumber": "7381440800349427",
            "totalSum": "2050.00",
            "kktOwner": "ИП ПЕТРОВ ПАВЕЛ ВЛАДИСЛАВОВИЧ",
            "kktOwnerInn": "213011202180",
            "key": "abakumovoleg@gmail.com|0|2026-01-30 18:57:00|21273|7381440800349427",
            "brandId": null,
            "receiveDate": "2026-01-30T19:02:12"
        },
        {
            "buyer": "abakumovoleg@gmail.com",
            "buyerType": "0",
            "createdDate": "2026-01-29T18:13:00",
            "fiscalDocumentNumber": "29535",
            "fiscalDriveNumber": "7380440902675340",
            "totalSum": "679.27",
            "kktOwner": "АО \"ВКУСВИЛЛ\"",
            "kktOwnerInn": "7734443270",
            "key": "abakumovoleg@gmail.com|0|2026-01-29 18:13:00|29535|7380440902675340",
            "brandId": 10339,
            "receiveDate": "2026-01-29T18:14:08"
        },
        {
            "buyer": "abakumovoleg@gmail.com",
            "buyerType": "0",
            "createdDate": "2026-01-29T17:46:00",
            "fiscalDocumentNumber": "167117",
            "fiscalDriveNumber": "7384440900877139",
            "totalSum": "679.27",
            "kktOwner": "АО \"ВКУСВИЛЛ\"",
            "kktOwnerInn": "7734443270",
            "key": "abakumovoleg@gmail.com|0|2026-01-29 17:46:00|167117|7384440900877139",
            "brandId": 10339,
            "receiveDate": "2026-01-29T17:48:08"
        }
    ],
    "brands": [
        {
            "id": 10898,
            "name": "Тинькофф мобайл",
            "description": "Услуги сотовой связи",
            "image": "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"
        },
        {
            "id": 10882,
            "name": "Яндекс.Такси",
            "description": "Заказ такси",
            "image": "/9j/4AAQSkZJRgABAQAAAQABAAD/4QAqRXhpZgAASUkqAAgAAAABADEBAgAHAAAAGgAAAAAAAABHb29nbGUAAP/bAEMAAgICAgIBAgICAgMCAgMDBgQDAwMDBwUFBAYIBwkICAcICAkKDQsJCgwKCAgLDwsMDQ4ODw4JCxAREA4RDQ4ODv/bAEMBAgMDAwMDBwQEBw4JCAkODg4ODg4ODg4ODg4ODg4ODg4ODg4ODg4ODg4ODg4ODg4ODg4ODg4ODg4ODg4ODg4ODv/AABEIAEYARgMBIgACEQEDEQH/xAAYAAEBAQEBAAAAAAAAAAAAAAAACQcKCP/EACsQAQAAAgUNAQEBAAAAAAAAAAADBAECBQYIBwkXGTlUVneTlbTS1BESIf/EABgBAQADAQAAAAAAAAAAAAAAAAABBwkF/8QAHhEBAAECBwAAAAAAAAAAAAAAAAECBQYVFlNUkdH/2gAMAwEAAhEDEQA/AKoAMk1+AAAAAAAAAAAAAAAAAANr0b2JvU71KvqaN7E3qd6lX1aCNKNB4P4VHU+qazS4bss+0b2JvU71KvqaN7E3qd6lX1aCGg8H8KjqfTNLhuyhfnB8YGUrChjIu3k9yeWNd217FtC6EC140a35SPGj0Rok1MwaatWmFGh0UVP5gVaaKPymn9pp/wBeEda/iG4RuD2qc+poGem2n1xuWsp588kCaEwfwaOp9Rmlw3ZU+1r+IbhG4Papz6jWv4huEbg9qnPqTBDQmEOFR1Ppmlw3JU+1r+IbhG4Papz6hMENCYQ4VHU+maXDcl3+ALEckABzA56bafXG5aynnzyQKv2em2n1xuWsp588kCAAAADv8AAABzA56bafXG5aynnzyQIAAAAA/9k="
        },
        {
            "id": 10339,
            "name": "Вкусвилл",
            "description": "Супермаркет",
            "image": "/9j/4AAQSkZJRgABAQEBLAEsAAD/2wBDAA0JCgsKCA0LCgsODg0PEyAVExISEyccHhcgLikxMC4pLSwzOko+MzZGNywtQFdBRkxOUlNSMj5aYVpQYEpRUk//2wBDAQ4ODhMREyYVFSZPNS01T09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT0//wAARCABGAEYDAREAAhEBAxEB/8QAGQABAAMBAQAAAAAAAAAAAAAAAAEDBAIF/8QALxAAAQQBAQQIBgMAAAAAAAAAAQACAwQREgUhMUETFEJRYYGhwSMyUnGRsVNygv/EABkBAQEBAQEBAAAAAAAAAAAAAAABAgMEBv/EACsRAAIBAgUCBQQDAAAAAAAAAAABAgMREiExQVEEoRMUIjJxM1Kx0ZHB8P/aAAwDAQACEQMRAD8A95fMnMIAgCAIAgCAIAgCAIAgCAIAgCAIAgCAIAgIcQ1pc4hoHMnAVSvoDFJtak1xayUzP+mFpefTcuy6eo82rfORjxImmk+1dfiPZttrPre0Afta8tJr0u/wWMm9jc6hba3Jgfjw3rL6aqlfCbszMuBAgKLFytVc1tieOIu+XUcZW4U5z9quRyUdS8EEAg5B5hZKdgR83OH+c+6qUeQcur0ZCDNG6UjvYPfK6RlGO7/BGluTJtKhs5rQYq8Or5TK7OfLgutOV/pwv85hyjES39pWGgx32RRkbuhjB3fcqvq56P8ARLyejM/R3M6jte8T/ZoH4wseZlx+f2Sz5LGdNkmeczE9otAPnjiuM5Ync0r7nSwU8Hb2DfrtdYigDoJAXytyN/L7r39J7HlfNaHGr7kZpb1mKtQNbXEw1TqAy7Q0EDXjngb/ADXSNKDlPFnn/kYcmkrGy3ZloGN9aWSzFZh6OIudq+JyPnn0XGnBVLqSs08/g3KTjpuJi6O3FTu7QlgiZAHCQSaTK/O/LvDuSNnBzhG7v/CI9bNlO0nMJ2aW3mFhbIBYmAcCMY3j0W6N/XePGSJPbMirbnjh2UyvG46o5GCMOOl5G4OPhzVnTi3UcnwFJq1juO1cn2dHWjnzcsTyDWOy1p3nwHAeay6dONRya9KS7lTk423LJ79i5TrR0naLUrS9+Ozo4jzduWYUo05Sc9F/ZXNtJR1PVp2G26kVhnCRucdx5j8ry1IOEnF7HWLurkWKxnc0iQM0j+Nrv2FYTw7ElG5wasxOTccSBjPQs4d3BXxI/b3ZnC+QaLXPrufIT1fJaA0NBceeB3K+K0mktS4dCH05ZG6ZbZeBydCw+yKolpHuw4t7h1OR4AfaLg3gDBGceiKqlou7JhfJPVpwQeuvyBgfCZu9FMcft7suF8kNqStOW2yD3iFg9lXUT27sYXyG05WuLm2yHHiRCwH9I6ieWHuxhfJfBGYo9BdqOSc6Q30G5c5SxO5pKyLFkBAEAQBAEAQBAEAQBAEAQBAEAQBAEAQBAEAQBAEAQBAEB//Z"
        }
    ],
    "hasMore": true
}

запрос на получение информации о чеке

postman request POST 'https://mco.nalog.ru/api/v1/receipt/fiscal_data' \
  --header 'Accept: application/json, text/plain, */*' \
  --header 'Accept-Language: ru,en;q=0.9' \
  --header 'Authorization: Bearer eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJ7XCJhdXRoVHlwZVwiOlwiU01TXCIsXCJsb2dpblwiOlwiNzk4NTI3NTkyMzJcIixcImlkXCI6MzYyNDE3NCxcImRldmljZUlkXCI6XCJ3YnkxYWRWcVdmWkhCVkZBelVmWDhcIixcInBob25lXCI6XCI3OTg1Mjc1OTIzMlwifSIsImV4cCI6MTc3MDU1ODYzNn0.g5TiMLH8FsttP0fyFNLryfbHreUugv0UjXks5Z53d7_v_eI5MeC3yhUrRPx6dMyQR2wGBFfdy2XnOREbTa697Q' \
  --header 'Connection: keep-alive' \
  --header 'Content-Type: application/json;charset=UTF-8' \
  --body '{"key":"abakumovoleg^@gmail.com|0|2026-01-29 17:46:00|167117|7384440900877139"}'

пример ответа

{
    "fiscalSign": "1738302144",
    "provisionSum": 0.00,
    "user": "АКЦИОНЕРНОЕ ОБЩЕСТВО \"ВКУСВИЛЛ\"",
    "machineNumber": null,
    "fiscalDriveNumber": "7384440900877139",
    "fiscalDocumentFormatVer": "4",
    "totalSum": 679.27,
    "taxationType": 1,
    "dateTime": "2026-01-29T17:46:00",
    "items": [
        {
            "sum": 65.27,
            "paymentType": 3,
            "nds": 4,
            "providerInn": null,
            "quantity": 65.27,
            "name": null,
            "productType": 10,
            "price": 1.00,
            "providerData": null,
            "paymentAgentData": null
        },
        {
            "sum": 614.00,
            "paymentType": 3,
            "nds": 12,
            "providerInn": null,
            "quantity": 614.0,
            "name": null,
            "productType": 10,
            "price": 1.00,
            "providerData": null,
            "paymentAgentData": null
        }
    ],
    "userInn": "7734443270  ",
    "fiscalDocumentNumber": 167117,
    "retailPlace": "https://vkusvill.ru/",
    "retailPlaceAddress": "Республика Башкортостан, 450057, г. Уфа, ул. Заки Валиди, 64/1",
    "cashTotalSum": 0.00,
    "prepaidSum": 0.00,
    "kktRegId": "0006895053041308    ",
    "paymentAgentType": null,
    "requestNumber": 5032,
    "nds10": null,
    "messageFiscalSign": null,
    "shiftNumber": 45,
    "fnsSite": null,
    "ecashTotalSum": 679.27,
    "operationType": 1,
    "creditSum": 0.00,
    "nds18": null,
    "receiptCode": null,
    "buyerAddress": "abakumovoleg@gmail.com",
    "operator": "Кассир",
    "internetSign": 1,
    "nds0": null,
    "ndsNo": null,
    "nds18118": null,
    "nds10110": 5.93,
    "amountsReceiptNds": {
        "amountsNds": [
            {
                "nds": 12,
                "ndsSum": 110.72
            }
        ]
    }
}