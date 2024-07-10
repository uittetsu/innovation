import cv2
import mediapipe as mp
import numpy as np
import time
import matplotlib.pyplot as plt
import socket


class Detection:
    def __init__(self):
        self.init_time = time.time()
        self.MAX_SPEED = 10
        self.MIN_SPEED = 1
        self.centroid_list = np.full((2, 3), 720, int)
        self.y_list = []
        self.y_integral = 0
        self.y_integral_list = []
        self.speed = 1

        # landmarkの繋がり表示用
        self.landmark_line_ids = [ 
            (0, 1), (1, 5), (5, 9), (9, 13), (13, 17), (17, 0),  # 掌
            (1, 2), (2, 3), (3, 4),         # 親指
            (5, 6), (6, 7), (7, 8),         # 人差し指
            (9, 10), (10, 11), (11, 12),    # 中指
            (13, 14), (14, 15), (15, 16),   # 薬指
            (17, 18), (18, 19), (19, 20),   # 小１
        ]

        self.mp_hands = mp.solutions.hands
        self.hands = self.mp_hands.Hands(
            max_num_hands=2,                # 最大検出数
            min_detection_confidence=0.7,   # 検出信頼度
            min_tracking_confidence=0.7     # 追跡信頼度
        )

        self.cap = cv2.VideoCapture(0)   # カメラのID指定


    def preprocess(self):
        self.img = cv2.flip(self.img, 1)          # 画像を左右反転
        self.img_h, self.img_w, _ = self.img.shape     # サイズ取得
        # 検出処理の実行
        self.results = self.hands.process(cv2.cvtColor(self.img, cv2.COLOR_BGR2RGB))


    def get_landmarks(self):
        self.lm_coodinate = np.empty((0, 4), int)
        # landmarkの繋がりをlineで表示
        for line_id in self.landmark_line_ids:
            # 1点目座標取得
            lm = self.hand_landmarks.landmark[line_id[0]]
            lm_pos1 = (int(lm.x * self.img_w), int(lm.y * self.img_h))
            # 2点目座標取得
            lm = self.hand_landmarks.landmark[line_id[1]]
            lm_pos2 = (int(lm.x * self.img_w), int(lm.y * self.img_h))
            # line描画
            cv2.line(self.img, lm_pos1, lm_pos2, (128, 0, 0), 3)
            # landmarkの番号表示
            cv2.putText(self.img, str(line_id[0]), (lm_pos1[0], lm_pos1[1] - 10), 
                        cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 3)
            if (line_id[0] -3) % 4 == 0:
                cv2.putText(self.img, str(line_id[1]), (lm_pos2[0], lm_pos2[1] - 10), 
                            cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 3)
            if line_id[1] - line_id[0] == 1:
                self.lm_coodinate = np.append(self.lm_coodinate, 
                                              np.array([[self.h_id, line_id[0], lm_pos1[0], lm_pos1[1]]]), axis=0)
                if line_id[1] % 4 == 0:
                    self.lm_coodinate = np.append(self.lm_coodinate, 
                                                  np.array([[self.h_id, line_id[1], lm_pos2[0], lm_pos2[1]]]), axis=0)


    def calc_centroids(self):
        centroid_x, centroid_y = int(np.mean(self.lm_coodinate[:, 2])), int(np.mean(self.lm_coodinate[:, 3]))
        self.centroid = [self.h_id, centroid_x, centroid_y]
        print(self.centroid)

    
    def correction(self, value):
        value = 2 * np.sqrt(value)
        return value

    def detect_waving_hands(self):
        self.THRESHOLD_LOW_INS = self.img_h / 10
        self.THRESHOLD_HIGH_INS = self.img_h / 3
        self.THRESHOLD_LOW_INS = self.correction(self.THRESHOLD_LOW_INS)
        self.THRESHOLD_HIGH_INS = self.correction(self.THRESHOLD_HIGH_INS)

        self.THRESHOLD_LOW_INT = 0
        self.THRESHOLD_HIGH_INT = 10000
        
        self.centroid_list[0] = self.centroid_list[1]
        self.centroid_list[1] = self.centroid
        
        diff_centroid_y = abs(self.centroid_list[0][2] - self.centroid_list[1][2])
        diff_centroid_y = self.correction(diff_centroid_y)
        self.y_list.append(diff_centroid_y)
        
        self.y_integral += diff_centroid_y
        self.y_integral_list.append(self.y_integral)

        self.hand_info = np.append(self.hand_info, self.y_integral) 

        # 瞬時値の積分値で判定
        if self.y_integral <= self.THRESHOLD_HIGH_INT:
            self.speed = (self.MAX_SPEED - self.MIN_SPEED) / self.THRESHOLD_HIGH_INT * self.y_integral + (self.MIN_SPEED * self.THRESHOLD_HIGH_INT - self.MAX_SPEED * self.THRESHOLD_LOW_INT) / (self.THRESHOLD_HIGH_INT - self.THRESHOLD_LOW_INT)
        else:
            self.speed = self.MAX_SPEED
        
        self.speed = max(1, self.speed)
                
        # 瞬時値で判定
        if (diff_centroid_y < self.THRESHOLD_LOW_INS) and (self.y_integral > self.THRESHOLD_LOW_INT):
            self.y_integral -= self.THRESHOLD_LOW_INS
        
        cv2.putText(self.img, ">>{:.2g}".format(self.speed), 
                            (10, 125), cv2.FONT_HERSHEY_SIMPLEX, 5, (0, 0, 255), 10)
        
        # return speed

    def display_landmark_points(self):
        # landmarkをcircleで表示
        z_list = [lm.z for lm in self.hand_landmarks.landmark]
        z_min = min(z_list)
        z_max = max(z_list)
        for lm in self.hand_landmarks.landmark:
            lm_pos = (int(lm.x * self.img_w), int(lm.y * self.img_h))
            lm_z_c = int((lm.z - z_min) / (z_max - z_min) * 255)
            # lm_z_s = int((z_max - lm.z) / (z_max - z_min) * 20)
            cv2.circle(self.img, lm_pos, 10, (255, lm_z_c, lm_z_c), -1)
        cv2.circle(self.img, (self.centroid[1], self.centroid[2]), 10, (0, 0, 255), -1)


    def display_hand_info(self):
        # 検出情報をテキスト出力
        # - テキスト情報を作成
        hand_texts = []
        for c_id, hand_class in enumerate(self.results.multi_handedness[self.h_id].classification):
            hand_texts.append("#%d-%d" % (self.h_id, c_id)) 
            hand_texts.append("- Index:%d" % (hand_class.index))
            hand_texts.append("- Label:%s" % (hand_class.label))
            hand_texts.append("- Score:%3.2f" % (hand_class.score * 100))
        # - テキスト表示に必要な座標など準備
        lm = self.hand_landmarks.landmark[0]
        lm_x = int(lm.x * self.img_w) - 50
        lm_y = int(lm.y * self.img_h) + 30
        lm_c = (0, 255, 255)
        font = cv2.FONT_HERSHEY_SIMPLEX
        # - テキスト出力
        for cnt, text in enumerate(hand_texts):
            cv2.putText(self.img, text, (lm_x, lm_y + 40 * cnt), font, 1, lm_c, 3)


    def detect_landmark(self):
        self.centroid = [0, self.img_w, self.img_h]
        self.hand_info = np.empty((0, 0), int)
        if self.results.multi_hand_landmarks:
            # 検出した手の数分繰り返し
            for self.h_id, self.hand_landmarks in enumerate(self.results.multi_hand_landmarks):
                # landmarkの繋がりをlineで表示
                self.get_landmarks()
                self.calc_centroids()
                self.detect_waving_hands()
                self.display_landmark_points()
                self.display_hand_info()
        else:
            self.detect_waving_hands()
            
        # return speed


    def display_image(self):
        # 画像の表示
        cv2.imshow("MediaPipe Hands", self.img)
    

    def plot(self):
        t = np.arange(1, len(self.y_list)+1)
        thresh_l_ins = []
        thresh_h_ins = []
        thresh_l_int = []
        thresh_h_int = []
        for i in range(len(t)):
            thresh_l_ins.append(self.THRESHOLD_LOW_INS)
            thresh_h_ins.append(self.THRESHOLD_HIGH_INS)
            thresh_l_int.append(self.THRESHOLD_LOW_INT)
            thresh_h_int.append(self.THRESHOLD_HIGH_INT)
        fig, axes = plt.subplots(2, 1, tight_layout = True)
        ax1, ax2 = axes
        ax1.plot(t, self.y_list)
        ax1.set_ylabel('displacement / frame') # y axis label
        # ax1.set_xlim(0, end_t) # x range
        ax1.set_ylim(0, self.correction(self.img_h)) # y range

        # Scales on the axis inside and also on the top and right
        ax1.xaxis.set_tick_params(which = 'both', direction = 'in', left = True, right=True)
        ax1.yaxis.set_tick_params(which = 'both', direction= 'in', bottom = True, top = True)

        # setting ticks
        # ax1.set_xticks([0, 1, 2, 3, 4, 5])
        ax1.tick_params(labelbottom = False) # Erase scale label on x-axis
      
        ax2.plot(t, self.y_integral_list)
        ax1.plot(t, thresh_h_ins)
        ax1.plot(t, thresh_l_ins)
        ax2.plot(t, thresh_h_int)
        ax2.plot(t, thresh_l_int)
        ax2.set_xlabel('deta of time series') # label of x axis
        ax2.set_ylabel('displacement / frame integration') # label of y axis
        # ax2.set_xlim(start_t, end_t)
        ax2.set_ylim(0, max(self.y_integral_list)) # ｙ range

        # Scales on the axis inside and also on the top and right
        ax2.xaxis.set_tick_params(which = 'both', direction = 'in', left = True, right = True)
        ax2.yaxis.set_tick_params(which = 'both', direction= 'in', bottom = True, top = True)
        # ax2.set_xticks([0, 1, 2, 3, 4, 5],
        #             ['0' , '1', '2', '3', '4', '5'])
        plt.show()


    def publish(self):
        # Unityに配信
        speed = str(self.speed)
        # client.sendto(speed.encode('utf-8'),(HOST,PORT))


    def main(self):
        if self.cap.isOpened():
            while True:
                # カメラから画像取得
                self.success, self.img = self.cap.read()
                # イメージが取得できない場合は処理をスキップ
                # print('ok')
                if not self.success:
                    print("イメージが取得できません")
                    # continue
                    break
                self.preprocess()
                self.detect_landmark()
                # self.display_image()
                # self.publish()
                print(self.speed, flush=True)
                # qかQかESCキー押下で終了
                # key = cv2.waitKey(1) & 0xFF
                # if key == ord('q') or key == ord('Q') or key == 0x1b:
                #     break
                # return self.speed
            # self.plot()
        self.cap.release()

# if __name__ == "__main__":
    # HOST = '127.0.0.1'
    # PORT = 50007

    # client = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    # print(client)
    # while True:
        # a = random.randrange(3)
    # result = str(a)
        # print(a)
    
    # connect.connect_unity()
    
D = Detection()
D.main()