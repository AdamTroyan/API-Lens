# 🎯 API Lens

**API Lens** is an advanced WPF-based API testing tool designed for developers and QA engineers.  
It allows you to send HTTP requests, visualize responses in a readable format, manage parameters, and save output easily.  

---

## 🛠️ Features

- **✅ Send POST Requests** with JSON to any API.
- **📊 Parameter Management**: Add, edit, and delete parameters using a friendly grid interface.
- **📄 Automatic JSON Formatting**: Readable and structured, including nested objects and arrays.
- **📝 Headers & Status Display**: Quickly view HTTP headers and status codes.
- **💾 Save Output**: Export formatted responses to `.txt` files.
- **🔗 Strict URL Validation**: RFC3986-compliant URL checking.
- **🚫 Ignore Certain Keys**: Automatically skips keys like `metadata`, `internal`, and `debug`.
- **🎨 User-Friendly UI**: Color-coded output, dynamic placeholder, and focus clearing on click outside input fields.

---

## 🎨 UI Overview

| UI Element | Description |
|------------|-------------|
| **URL Box** | Input field for the API URL |
| **Placeholder** | Dynamic text displayed when the field is empty |
| **Settings Button** | Opens the window to configure request parameters |
| **Parameters Grid** | Lists parameters with `Key` and `Value` columns |
| **Send Button** | Sends the request to the API |
| **Result Box** | Displays formatted output with colors: <br>- `Status` → Green<br>- `HEADERS` → Cyan<br>- `PARAMETERS` → Yellow<br>- `BODY` → Orange/Light Blue |
| **Save Button** | Saves the output as a `.txt` file |

---

## 🚀 How to Use

1. **Enter a valid API URL** in the URL field.
2. Click **Settings** to add or modify request parameters.
3. Click **Send** to execute the request.
4. The output will appear in the main panel with color-coded sections.
5. Optionally, save the output using **Save**.

---

### 🌈 Output Colors

- **Status:** ![#32CD32](https://via.placeholder.com/15/32CD32/000000?text=+) Green
- **HEADERS:** ![#00FFFF](https://via.placeholder.com/15/00FFFF/000000?text=+) Cyan
- **PARAMETERS:** ![#FFFF00](https://via.placeholder.com/15/FFFF00/000000?text=+) Yellow
- **BODY:** ![#FFA500](https://via.placeholder.com/15/FFA500/000000?text=+) Orange
- **Value lines:** ![#ADD8E6](https://via.placeholder.com/15/ADD8E6/000000?text=+) Light Blue

---

## 🖼️ Example

**Request Parameters:**

| Key       | Value      |
|-----------|-----------|
| api_key   | 12345     |
| user_id   | 6789      |

**API Response JSON:**
```json
{
    "status": "success",
    "data": {
        "id": 123,
        "name": "Sample Item"
    }
}
```

## Formatted Output in API Lens:
```xml
Status: 200 OK
--------------------------------------------------
HEADERS:
Content-Type: application/json
...

PARAMETERS:
api_key: 12345
user_id: 6789
...

BODY:
data:
    id: 123
    name: Sample Item
```

---

## 💡 Tips

Avoid URLs with spaces or invalid characters.

Empty parameters will not be sent to the API.

Large JSON responses are formatted with proper indentation.

The parameter window allows quick copy and modification of all values in real-time.

---

## 📥 Installation

**Clone the repository:**

```git clone https://github.com/YourUsername/API_Lens.git```


Open the solution in Visual Studio 2022+.

Build and run the project.

Requires .NET 7+.

---

## 📝 Contributing

Contributions are welcome:

UI/UX improvements

Support for GET requests

Additional JSON validations

Adding authentication support

Fork → Create Branch → Pull Request ✅

---

## 📄 License

MIT License – LICENSE

Created with ❤️ for API testing and visualization
