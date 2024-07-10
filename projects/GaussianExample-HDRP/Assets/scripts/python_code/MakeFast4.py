import cv2
import glob
import os
import tqdm
import math

# scale = 3
W = 1280
H = 720
FPS = 30
min_scale = 0
max_scale = 10

video_paths = glob.glob(os.path.join("/GaussianExample-HDRP/Assets/movie/Original_video", "*.mp4"))
video_paths += glob.glob(os.path.join("/Users/uchiyamaittetsu/Desktop/innovation/unity/share/innovation/projects/GaussianExample-HDRP/Assets/movie/Original_video", "*.MOV"))
video_paths = ["/Users/inouetomoki/innovation/projects/GaussianExample-HDRP/Assets/movie/Original_video/豊田講堂→山の上.MOV"]
video_paths2 = glob.glob(os.path.join("..", "..", "movie", "Original_video", "*"))
print(video_paths2)

for video_path in video_paths:
    video_name = video_path.split(".")[0].split("/")[-1]
    print("processing :", video_name)
    
    
    cap = cv2.VideoCapture(video_path)

    frame_count = int(cap.get(cv2.CAP_PROP_FRAME_COUNT))
    
    # fourcc 
    
    video_list = list()
    save_dir = os.path.join("..", "..", "movie", video_name)
    os.makedirs(save_dir, exist_ok=True)
    
    for n in range(min_scale, max_scale):
        speed_factor = 2 * (1.25 ** n)
        frame_interval = int(math.ceil(speed_factor))
        print(speed_factor, frame_interval)
        
        # save_path = f"../../movie/{video_name}/{video_name}_{scale}x.mp4"
        save_path = os.path.join(save_dir, f"{video_name}_{speed_factor:.2f}x.mp4")
        
        video = cv2.VideoWriter(save_path, cv2.VideoWriter_fourcc(*'mp4v'), FPS, (W, H))
        video_list.append(video)

    # i = 0
    for i in tqdm.tqdm(range(frame_count)):
        ret, frame = cap.read()
        
        
        if not ret:
            break
        
        frame = cv2.resize(frame, (W, H))
        
        for idx, n in enumerate(range(min_scale, max_scale)):
            speed_factor = 2 * (1.25 ** n)
            frame_interval = int(math.ceil(speed_factor))
            if i % frame_interval == 0:
                video_list[idx].write(frame)
        
        # i += 1

    video.release()
    # new_video.release()