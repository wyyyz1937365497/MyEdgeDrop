import requests

# 目标URL

# 要上传的文件路径
file_path = "C://Users//wyy19//Downloads//CodeGeeXVS.vsix"

# 打开文件
with open(file_path, 'rb') as file:
    # 发送POST请求，上传文件
    response = requests.post("http://127.0.0.1:19255/upload", files={'file': file})

# 检查响应状态码
if response.status_code == 200:
    # 打印响应内容
    print(response.text)
else:
    print(f'请求失败，状态码：{response.status_code}')
