import cv2
import os

scale = 3

video_path = "/Users/uchiyamaittetsu/Desktop/innovation/unity/share/innovation/projects/GaussianExample-HDRP/Assets/movie/Original_video/石碑→豊田講堂.MOV"
save_path = "/Users/uchiyamaittetsu/Desktop/innovation/unity/share/innovation/projects/GaussianExample-HDRP/Assets/movie/" + video_path.split(".")[0].split("/")[-1] + f"_{scale}x" + ".mp4"

if os.path.exists(save_path):
    exit()

video = cv2.VideoCapture(video_path)

fps = int(video.get(cv2.CAP_PROP_FPS))
width = int(video.get(cv2.CAP_PROP_FRAME_WIDTH))
height = int(video.get(cv2.CAP_PROP_FRAME_HEIGHT)
             )
fourcc = cv2.VideoWriter_fourcc(*'mp4v')
new_video = cv2.VideoWriter(save_path, fourcc, fps, (width, height))

i = 0
while True:
    ret, frame = video.read()
    
    if not ret:
        break
    
    if i % scale == 0:
        new_video.write(frame)
    
    i += 1

video.release()
new_video.release()