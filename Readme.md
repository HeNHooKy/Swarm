***To run this project
1. Setup your database with flatmeters.sql
2. Add connection string and checks frequency of meter to appsetting.json
```
{
  "ConnectionStrings": {
    "Model": "Server=localhost\\SQLEXPRESS;Database=Model;Trusted_Connection=True;"
  },
  "Meter": {
    "ChecksFrequencyInDays": 30
  },
}
```
3. bulid!
4. run!