import uvicorn
from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.responses import StreamingResponse, JSONResponse
import os

app = FastAPI()

def write_to_file(new_line):
    with open('log.txt', 'a', encoding='gb18030') as file:
        # 写入新的一行内容
        file.write("\n"+new_line)
    print("新行已成功追加到文件中。")

def read_from_file():
    with open("log.txt", 'r', encoding="gb18030",errors = 'ignore') as file:
        file.seek(0, 2)  # 移动到文件末尾
        pos = file.tell()  # 获取当前位置（文件末尾）
        while pos > 0:
            pos -= 1
            file.seek(pos, 0)  # 移动到新的位置
            if file.read(1) == '\n':  # 检查是否是换行符
                break
        last_line = file.readline()  # 读取最后一行
        print("最后一行内容是：", last_line.strip())
    return last_line.strip()

@app.get("/get_message")
def get_message():
    new_message = read_from_file()
    return new_message

@app.get("/upload_message")
def upload_message(message: str):
    write_to_file(message)
    return {"message": "Message uploaded successfully"}


# 确保上传目录存在
UPLOAD_DIR = "uploads"
os.makedirs(UPLOAD_DIR, exist_ok=True)

@app.get("/download/{file_path:path}")
async def download_file(file_path: str):
    full_path = os.path.join(UPLOAD_DIR, file_path)
    if not os.path.exists(full_path):
        raise HTTPException(status_code=404, detail="File not found")
    
    def iterfile():
        with open(full_path, "rb") as file:
            while chunk := file.read(8192):
                yield chunk
    
    filename = os.path.basename(full_path)
    # 使用 RFC 5987 编码文件名
    encoded_filename = filename.encode('utf-8').hex()
    content_disposition = f"attachment; filename*=utf-8''{encoded_filename}"
    
    return StreamingResponse(
        iterfile(),
        media_type="application/octet-stream",
        headers={"Content-Disposition": content_disposition}
    )
@app.post("/upload/")
async def upload_file(file: UploadFile = File(...)):
    # 构建完整的文件路径
    full_path = os.path.join(UPLOAD_DIR, file.filename)
    
    # 保存文件
    with open(full_path, "wb") as buffer:
        while chunk := await file.read(8192*4):  # 每次读取8KB
            buffer.write(chunk)
    
    return {"message": "File uploaded successfully", "filename": file.filename}
@app.get("/list_files")
def list_files():
    folder_path = "./uploads"  # 替换为你的文件夹路径
    files = os.listdir(folder_path)
    return files

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=19255)