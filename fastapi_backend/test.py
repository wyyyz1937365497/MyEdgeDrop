import requests

def download_file_from_api(server_url, file_path, save_path):
    """
    从FastAPI服务器下载文件。

    :param server_url: 服务器的基地址，例如 http://127.0.0.1:8000
    :param file_path: 要下载的文件的路径
    :param save_path: 文件保存到本地的路径
    """
    # 构造完整的下载URL
    download_url = f"{server_url}/download/{file_path}"

    # 发起GET请求
    with requests.get(download_url, stream=True) as response:
        # 检查响应状态码
        if response.status_code == 200:
            # 以写入二进制的方式打开保存文件
            with open(save_path, "wb") as file:
                # 分块写入文件
                for chunk in response.iter_content(chunk_size=8192):
                    file.write(chunk)
            print(f"文件已保存到: {save_path}")
        else:
            # 打印错误信息
            print(f"下载失败，状态码: {response.status_code}, 错误信息: {response.text}")

# 使用示例
server_url = "http://127.0.0.1:19255"  # 替换为您的服务器地址
file_path = "example.txt"            # 替换为服务器上的文件路径
save_path = "downloaded_example.txt"  # 替换为希望保存到本地的路径

download_file_from_api(server_url, file_path, save_path)
