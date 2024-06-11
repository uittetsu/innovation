import cv2

scale = 5

video_path = "/Users/uchiyamaittetsu/Desktop/innovation/unity/share/innovation/projects/GaussianExample-HDRP/Assets/movie/豊田講堂→駅.mp4"
save_path = video_path.split(".")[0] + f"_{scale}x" + ".mp4"

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