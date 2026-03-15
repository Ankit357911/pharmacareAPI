# API Examples

All protected endpoints require:

`Authorization: Bearer <JWT_TOKEN>`

## 1) Login

`POST /api/Users/login`

Request:

```json
{
  "mobileNumber": "9800000000",
  "password": "YourPassword123"
}
```

Response:

```json
{
  "token": "<jwt-token>"
}
```

## 2) Create Staff Account (Admin)

`POST /api/Users/staff`

Request:

```json
{
  "fullName": "John Staff",
  "mobileNumber": "9811111111",
  "email": "john.staff@example.com",
  "password": "StrongPass123"
}
```

## 3) Upsert Medicine

`POST /api/Medicines/upsert`

Request:

```json
{
  "medicineName": "Paracetamol",
  "categoryName": "Pain Relief",
  "purchaseRate": 20.00,
  "sellingRate": 25.00,
  "stock": 100,
  "manufacturingDate": "2026-01-01",
  "expiryDate": "2028-01-01"
}
```

## 4) Recent Transactions

`GET /api/Transactions/recent?take=8`

## 5) Current Earnings Summary

`GET /api/Earnings/summary/current`
