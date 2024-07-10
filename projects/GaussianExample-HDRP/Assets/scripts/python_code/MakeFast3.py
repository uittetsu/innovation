import cv2
import glob
import os
import tqdm


# scale = 3
min_scale = 2
max_scale = 11

video_paths = glob.glob(os.path.join("/GaussianExample-HDRP/Assets/movie/Original_video", "*.mp4"))
video_paths += glob.glob(os.path.join("/Users/uchiyamaittetsu/Desktop/innovation/unity/share/innovation/projects/GaussianExample-HDRP/Assets/movie/Original_video", "*.MOV"))
video_paths = ["/Users/inouetomoki/innovation/projects/GaussianExample-HDRP/Assets/movie/Original_video/豊田講堂→山の上.MOV"]
video_paths2 = glob.glob(os.path.join("..", "..", "movie", "Original_video", "*"))
print(video_paths2)

for video_path in video_paths:
    video_name = video_path.split(".")[0].split("/")[-1]
    print("processing :", video_name)
    
    
    cap = cv2.VideoCapture(video_path)

    # fps = int(video.get(cv2.CAP_PROP_FPS))
    # width = int(video.get(cv2.CAP_PROP_FRAME_WIDTH))
    # height = int(video.get(cv2.CAP_PROP_FRAME_HEIGHT))
    frame_count = int(cap.get(cv2.CAP_PROP_FRAME_COUNT))
    
    # fourcc 
    
    video_list = list()
    save_dir = os.path.join("..", "..", "movie", video_name)
    os.makedirs(save_dir, exist_ok=True)
    
    for scale in range(min_scale, max_scale):
        
        # save_path = f"../../movie/{video_name}/{video_name}_{scale}x.mp4"
        save_path = os.path.join(save_dir, f"{video_name}_{scale}x.mp4")
        
        video = cv2.VideoWriter(save_path, cv2.VideoWriter_fourcc(*'mp4v'), 30, (1280, 720))
        video_list.append(video)

    # i = 0
    for i in tqdm.tqdm(range(frame_count)):
        ret, frame = cap.read()
        
        if not ret:
            break
        
        frame = cv2.resize(frame, (1280, 720))
        
        for idx, scale in enumerate(range(min_scale, max_scale)):
            if i % scale == 0:
                video_list[idx].write(frame)
        
        # i += 1

    video.release()
    # new_video.release()