using Datacenter.Model.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datacenter.Model.Utils
{
    public class FuelTest : ISerializerModal
    {
        /// <summary>
        /// Số lượng điểm kế tiếp tối đa cần kiểm tra
        /// </summary>
        public const int MAXNEXT = 30;

        /// <summary>
        /// Danh sách các ứng cử sự kiện cần xác minh theo điểm kế tiếp
        /// </summary>
        public List<FuelEventCandidate> Candidates = new List<FuelEventCandidate>();

        /// <summary>
        /// Sự kiện đang xem xét
        /// </summary>
        public FuelEventCandidate Current = new FuelEventCandidate();


        /// <summary>
        /// Danh sách 3 điểm cần test, điểm 1 sẽ là điểm xử lý, điểm 2 và 3 còn trong quá trình kiểm tra có phải nhiễu hay không.
        /// Khái niệm: nếu một điểm mà không tồn tại điểm bên cạnh có giá trị gần nó thì đó là điểm nhiễu sẽ bị loại bõ
        /// </summary>
        private List<FuelPoint> CandidatePoints = new List<FuelPoint>(3);//version 19

        /// <summary>
        /// Sử dùng để loại trùng ( không xử lý điểm có giá trị trùng) : cần giải quyết giá trị to
        /// </summary>
        private FuelPoint PrevPoint = new FuelPoint();//version 19


        /// <summary>
        /// Giá trị tối đa của bình chứa : nhằm tính ra các ngưỡng tỉ lệ
        /// </summary>
        int fuelCapicity;
        float noiseThreadhold = 2000;//2000ml
        private int FuelCapicity
        {
            get
            {
                return fuelCapicity;
            }
            set
            {
                fuelCapicity = value;
                noiseThreadhold = fuelCapicity / 10f;
                Current.FuelCapicity = fuelCapicity;
            }
        }

        /// <summary>
        /// Xử lý và trả về điểm cần xử lý sự kiện, trả về null nếu không có
        /// </summary>
        /// <param name="newval"></param>
        /// <param name="newtime"></param>
        /// <returns></returns>
        private FuelPoint getProcessPoint(float lat, float lng, float newval, DateTime newtime)
        {
            //Thêm 1 điểm mới vô cuối
            CandidatePoints.Add(new FuelPoint() { Time = newtime, Val = newval, Lat = lat, Lng = lng });

            //chưa đủ 3 điểm thì không xét
            if (CandidatePoints.Count < 3) return null;

            //kiểm tra noise 
            float noise = Math.Abs(CandidatePoints[2].Val - CandidatePoints[1].Val);
            if(noise > noiseThreadhold)
            {
                CandidatePoints.RemoveAt(1);//remove middle point
                return null;
            }

            //nếu đủ 3 điểm và không có nhiễu thì trả về điểm đầu tiên và xóa nó ra khỏi CandidatePoints
            FuelPoint ret = CandidatePoints[0];
            CandidatePoints.RemoveAt(0);
            return ret;
        }


        public List<FuelEventCandidate> CheckAndGetEvents(float lat, float lng,float newval, DateTime newtime,int fuelCapicity, DeviceType dtype)
        {
            List<FuelEventCandidate> ret = null;

            //Cập nhật khả năng bình chứa cho sự kiện đang xét
            if (FuelCapicity != fuelCapicity) FuelCapicity = fuelCapicity;

            //Kiểm tra trùng theo giá trị 
            if (PrevPoint.Val == newval)
            {
                //nếu Candidates quá 12 giờ thì mặc định đây là sự kiện thật mà không cần xét gì thêm
                for (int i = Candidates.Count - 1; i >= 0; i--)
                {
                    FuelEventCandidate Candidate = Candidates[i];
                    if ((newtime -  Candidate.FistTime).Hours>12)
                    {
                        if (ret == null) ret = new List<FuelEventCandidate>(1);
                        ret.Add(Candidate);
                        Candidates.RemoveAt(i);
                        Current.ResetPrev(Candidate);
                    }
                }

                //nếu không thì bõ qua không xử lý giá trị trùng
                Current.LastOfPrevTime = newtime;//cập nhật thời gian nếu có trùng
                return ret;
            }

            //giữ lại giá trị đã xử lý
            PrevPoint.Update(lat, lng, newval, newtime);

            //Lấy điểm xử lý
            FuelPoint testpoint = getProcessPoint(lat,lng,newval, newtime);
            if (testpoint == null) return ret;

            //System.Diagnostics.Debug.WriteLine(testpoint.Time.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + testpoint.Val.ToString());

            //Xử lý testpoint và trả về kết quả nếu có
            newval = testpoint.Val;newtime = testpoint.Time;lat = testpoint.Lat;lng = testpoint.Lng;

            //nếu có sự kiện giả thì reset lại current và lấy prev theo cái cuối cùng
            bool hasFakeEvent = false;
            for (int i = Candidates.Count - 1; i >= 0; i--)
            {
                FuelEventCandidate Candidate = Candidates[i];

                //tăng số lần kiểm tra
                Candidate.IncreaseNextCounter(newval);

                //kiểm tra sự kiện giả
                bool isFakeEvent = Candidate.SumDelta > 0 && Candidate.FirstContains(newval) //nếu chứa điểm mới thì đây là sự kiện giả
                    || Candidate.PrevContains(newval);//Kiểm tra điểm kế tiếp không trùng với điểm trước đó trong Prevs của Candidate

                if(isFakeEvent)
                {
                    Candidates.RemoveAt(i);
                    if (!hasFakeEvent)
                    {
                        hasFakeEvent = true;
                        Current.Reset(newval, newtime, true);
                        Current.Prevs = Candidate.Prevs;
                    }
                }
                //nếu quá N lân kiểm tra ma vẫn k thấy chưa điểm mới thì đây là sự kiện thật
                //Hoặc quá 12 giờ kể từ lúc xét sự kiện
                else if (Candidate.NextCounter > MAXNEXT)
                {
                    if (ret == null) ret = new List<FuelEventCandidate>(1);
                    ret.Add(Candidate);
                    Candidates.RemoveAt(i);
                    Current.ResetPrev(Candidate);
                }
            }

            //Kiểm tra điểm mới
            if (!hasFakeEvent && Current.Check(lat,lng,newval, newtime,dtype))
            {
                //Thêm sự kiện nào kết quả
                FuelEventCandidate e = Current.Copy();
                Candidates.Add(e);

                //Reset lại sử dụng cho lần sau
                Current.PrevAdd(Current.PrevValue);
                Current.Reset(newval, newtime, true);
            }

            return ret;
        }

        public void Deserializer(BinaryReader stream, int version)
        {
            Current.Deserializer(stream, version);
            int n = stream.ReadInt32();
            for (int i = 0; i < n; i++)
            {
                FuelEventCandidate obj = new FuelEventCandidate();
                obj.Deserializer(stream, version);
                Candidates.Add(obj);
            }

            //version 11
            if (version == 11)
            {
                stream.ReadInt64();
                stream.ReadDouble();
            }

            //version 19 
            if (version >= 19)
            {
                PrevPoint.Deserializer(stream, version);
                n = stream.ReadInt32();
                for (int i = 0; i < n; i++)
                {
                    FuelPoint obj = new FuelPoint();
                    obj.Deserializer(stream, version);
                    CandidatePoints.Add(obj);
                }
            }

        }

        public void Serializer(BinaryWriter stream)
        {
            Current.Serializer(stream);
            stream.Write(Candidates.Count);
            foreach (var item in Candidates)
                item.Serializer(stream);

            //version 19
            PrevPoint.Serializer(stream);
            stream.Write(CandidatePoints.Count);
            foreach (var item in CandidatePoints)
                item.Serializer(stream);
        }

        /// <summary>
        /// Lấy giá trị dầu trước khi có sự kiện, -1 nếu không tồn tại
        /// </summary>
        public float FuelOfBeginingEvent
        {
            get
            {
                float ret = -1;

                ////lấy giá trị 
                //if (Candidates.Count > 0)
                //    ret = Candidates[0].FistValue;
                //else 
                if (Current.Counter > 0)
                    ret = Current.FistValue;

                //Giá trị không hợp lệ
                if (FuelCapicity > 0 &&  ret > FuelCapicity) ret = -1;

                return ret;
            }
        }

        //20 bytes
        public class FuelPoint
        {
            public float Lat;//4
            public float Lng;//4
            public float Val;//4
            public DateTime Time;//8

            public void Update(float lat,float lng,float val,DateTime time)
            {
                Lat = lat;
                Lng = lng;
                Val = val;
                Time = time;
            }

            public void Deserializer(BinaryReader stream, int version)
            {
                Lat = stream.ReadSingle();
                Lng = stream.ReadSingle();
                Val = stream.ReadSingle();
                Time = DateTime.FromBinary(stream.ReadInt64());
            }

            public void Serializer(BinaryWriter stream)
            {
                stream.Write(Lat);
                stream.Write(Lng);
                stream.Write(Val);
                stream.Write(Time.ToBinary());
            }
        }

    }

    public class FuelEventCandidate : IComparable<FuelEventCandidate>, ISerializerModal
    {
        public const int MAXPREV = 6;//lưu 6 giá trị trước đó

        /// <summary>
        /// Thời điểm mốc thay đổi ban đầu
        /// </summary>
        public DateTime FistTime;

        /// <summary>
        /// Giá trị mốc thay đổi ban đầu
        /// </summary>
        public float FistValue;

        /// <summary>
        /// Thời điểm mốc liền trước đó
        /// hoặc Thời điểm kết thúc sự kiện
        /// </summary>
        public DateTime PrevTime;

        /// <summary>
        /// Thời điểm cuối cùng của PrevTime nếu như giá trị bị trùng
        /// </summary>
        public DateTime LastOfPrevTime;

        /// <summary>
        /// Giá trị liền trước đó
        /// hoặc Giá trị kết thúc sự kiện
        /// </summary>
        public float PrevValue;

        /// <summary>
        /// độ lệch giá trị tích lũy
        /// </summary>
        public float SumDelta = 0;

        /// <summary>
        /// số điểm tích lũy
        /// </summary>
        public short Counter = 0;

        /// <summary>
        /// thời gian tích lũy khi có sự thay đổi tăng giảm tính bằng giây
        /// </summary>
        public int CounterSeconds = 0; //version 13

        /// <summary>
        /// số điểm tích lũy sau khi ứng cử (candidate)
        /// </summary>
        public byte NextCounter = 0;

        /// <summary>
        /// Danh sách các điểm có ý nghĩa trước đó
        /// </summary>
        public List<float> Prevs = new List<float>(MAXPREV);


        /// <summary>
        /// Giá trị nhiễu liền trước đó,
        /// </summary>
        public float PrevNoise = -1;


        /// <summary>
        /// độ lệch lớn nhất
        /// </summary>
        public float MaxDelta = 0;//version 15

        /// <summary>
        /// Thời điểm độ lệch lớn nhất
        /// </summary>
        public DateTime MaxDeltaTime;//version 15

        /// <summary>
        /// Tọa độ lat ứng với độ lệch lớn nhất
        /// </summary>
        public float MaxDeltaLat;//version 15

        /// <summary>
        /// Tọa độ lng ứng với độ lệch lớn nhất
        /// </summary>
        public float MaxDeltaLng;//version 15


        /// <summary>
        /// Giá trị tối đa của bình chứa : nhằm tính ra các ngưỡng tỉ lệ
        /// </summary>
        int fuelCapicity;
        double NoiseCapicity;
        double NoiseDelta;
        public int FuelCapicity
        {
            get
            {
                return fuelCapicity;
            }
            set
            {
                fuelCapicity = value;

                //Giá trị lưu lại bằng tổng bình / 6
                NoiseCapicity = Math.Min(Math.Max((double)fuelCapicity / 7, 10000), 20000);//tối thiểu 10000ml, tối đa 20000ml

                //Giá trị tối đa noise / MAXPREV
                NoiseDelta = NoiseCapicity / MAXPREV;
            }
        }

        int limitAdd
        {
            get
            {
                //xét >=10 lít, nếu bình 190 lít trỡ lên thì chỉ xét khi THÊM 19 lít trở lên
                return fuelCapicity > 190000 ? 19000 : 10000; 
            }
        }

        int limitSub
        {
            get
            {
                //xét <=-9 lít, nếu bình 380 lít trỡ lên thì chỉ xét khi HÚT 20 lít trỡ lên
                return fuelCapicity > 380000 ? 19999 : 9000; 
            }
        }

        public FuelEventCandidate Copy()
        {
            FuelEventCandidate ret = MemberwiseClone() as FuelEventCandidate;
            ret.Prevs = new List<float>(Prevs);
            return ret;
        }

        public void PrevAdd(float prev)
        {
            //if (!Prevs.Contains(prev)) Prevs.Add(prev);
            if (!PrevContains(prev)) Prevs.Add(prev);
            while (Prevs.Count > MAXPREV
                || (Prevs.Count > 0 && Math.Abs(Prevs[0] - prev) > NoiseCapicity)
                )
                Prevs.RemoveAt(0);
        }

        /// <summary>
        /// Điểm đang xét nằm trong danh sách k điểm trước đó
        /// </summary>
        /// <param name="newval"></param>
        /// <returns></returns>
        public bool PrevContains(float newval)
        {
            //foreach(float testval in Prevs)
            //{
            //    if (Math.Abs(testval - newval) < NOISE_THRESHOLD) return true;
            //}
            //return false;
            return Prevs.Count > 0 && newval > Prevs.Min() - NoiseDelta && newval < Prevs.Max() + NoiseDelta;
        }

        /// <summary>
        /// Điểm đang xét trùng với điểm đầu tiên
        /// </summary>
        /// <param name="newval"></param>
        /// <returns></returns>
        public bool FirstContains(float newval)
        {
            return Math.Abs(FistValue - newval) < NoiseDelta;
        }


        /// <summary>
        /// Hàm này sẽ loại bõ những giá trị trước đó Prevs đã tòn tại trước khi có sự kiện. Được gọi khi nhận ra sự kiện.
        /// Lưu ý: nếu vẫn bị lỗi, có thể thêm tất cả giá trị gặp được cho đủ MAXPREV (hiện tại chưa chặn)
        /// </summary>
        /// <param name="candidate"></param>
        public void ResetPrev(FuelEventCandidate candidate)
        {
            if (Prevs.Count <= 0) return;

            //find first prev in candidate list
            int idxCandidateRemoveFrom = -1;
            for (int i = candidate.Prevs.Count - 1; i >= 0; i--)
            {
                if (candidate.Prevs[i] == Prevs[0])
                {
                    idxCandidateRemoveFrom = i;
                    break;
                }
            }

            if (idxCandidateRemoveFrom < 0) return;

            //find old last index in prevs
            int OldLastIdx;
            for (OldLastIdx = 0; OldLastIdx < Prevs.Count; OldLastIdx++)
            {
                if (idxCandidateRemoveFrom >= candidate.Prevs.Count)
                    break;

                if (candidate.Prevs[idxCandidateRemoveFrom++] != Prevs[OldLastIdx])
                {
                    OldLastIdx = -1;
                    break;
                }
            }

            if (OldLastIdx > 0)
                Prevs.RemoveRange(0, OldLastIdx);
        }


        /// <summary>
        /// Tăng giá trị test điểm kế
        /// </summary>
        /// <param name="newval"></param>
        public void IncreaseNextCounter(float newval)
        {
            NextCounter++;
        }

        /// <summary>
        /// Reset lại thông tin
        /// </summary>
        /// <param name="newval">giá trị hiện hành</param>
        /// <param name="newtime">thời gian hiện hành</param>
        /// <param name="reset">Reset không giữ lại giá trị trước đó</param>
        public void Reset(float newval, DateTime newtime, bool reset)
        {
            PrevNoise = -1;//reset lại giá trị nhiễu
            if (reset)
            {
                SumDelta = 0;
                Counter = 0;
                CounterSeconds = 0;
                PrevValue = FistValue = newval;
                PrevTime = FistTime = newtime;
            }
            else
            {
                PrevAdd(PrevValue);

                EnsureLastOfPrevTime();
                CounterSeconds = (int)(newtime - LastOfPrevTime).TotalSeconds;

                SumDelta = newval - PrevValue;
                Counter = 1;
                FistValue = PrevValue;
                FistTime = PrevTime;
                PrevValue = newval;
                PrevTime = newtime;
            }
            MaxDelta = MaxDeltaLat = MaxDeltaLng = 0;
            MaxDeltaTime = PrevTime;
            EnsureLastOfPrevTime();
        }

        private void EnsureLastOfPrevTime()
        {
            //make sure LastOfPrevTime alway >= PrevTime
            if (LastOfPrevTime < PrevTime) LastOfPrevTime = PrevTime;//LastOfPrevTime does not be saved to disk
        }

        private void UpdatePrev(float newval, DateTime newtime)
        {
            EnsureLastOfPrevTime();
            CounterSeconds += (int)(newtime - LastOfPrevTime).TotalSeconds;

            PrevTime = newtime;
            PrevValue = newval;
            EnsureLastOfPrevTime();
        }

        /// <summary>
        /// Kiểm tra sự kiện
        /// </summary>
        /// <param name="lat">lat</param>
        /// <param name="lng">lng</param>
        /// <param name="newval">giá trị hiện hành</param>
        /// <param name="newtime">thời gian hiện hành</param>
        /// <returns></returns>
        public bool Check(float lat, float lng, float newval, DateTime newtime, DeviceType dtype)
        {
            //Chưa có giá trị ban đầu --> khởi tạo
            if (!FistTime.IsValidDatetime())
            {
                Reset(newval, newtime, true/*,false*/);
                PrevAdd(newval);
                return false;
            }

            //Bõ qua các giá trị nhiễu
            if (newval == PrevNoise)
                return false;

            //Tính độ lệch
            float delta = newval - PrevValue;
            bool isEvent = false;
            if (delta > 0)//kiểm tra tăng
            {
                if (SumDelta >= 0)
                {
                    SumDelta += delta;
                    Counter++;
                    UpdatePrev(newval, newtime);

                    //Tìm độ lệch lớn nhất lấy đó làm thời điểm sự kiện
                    if (MaxDelta < delta)
                    {
                        MaxDelta = delta;
                        MaxDeltaTime = newtime;
                        MaxDeltaLat = lat;
                        MaxDeltaLng = lng;
                    }

                    return false;
                }
                else//đão chiều
                {
                    //độ lệch tích lũy vượt mốc
                    isEvent = SumDelta + delta <= -limitSub
                        && (Counter > 1 || (newtime - FistTime).TotalSeconds > 60);
                }
            }
            else if (delta < 0)//kiểm tra giảm
            {
                if (SumDelta <= 0)
                {
                    //Nếu tốc độ rút quá nhanh thì có thể do nhiễu nên không xét 
                    if (dtype!=DeviceType.OilVehicle 
                        &&  (newtime - PrevTime).TotalSeconds < 30) //thời gian 2 chu kì < 30 giây
                    {
                        if (delta < -limitSub) //đây là sự kiện giả : reset lại giá trị
                        {
                            PrevNoise = newval;
                            return false;
                        }
                    }

                    SumDelta += delta;
                    Counter++;
                    UpdatePrev(newval, newtime);

                    //Tìm độ lệch lớn nhất lấy đó làm thời điểm sự kiện
                    if (MaxDelta > delta)
                    {
                        MaxDelta = delta;
                        MaxDeltaTime = newtime;
                        MaxDeltaLat = lat;
                        MaxDeltaLng = lng;
                    }

                    return false;
                }
                else//đão chiều
                {
                    //độ lệch tích lũy vượt mốc
                    isEvent = SumDelta + delta >= limitAdd;
                }
            }

            if (!isEvent)
            {
                Reset(newval, newtime, Math.Abs((SumDelta + delta) / SumDelta) < 0.5f/*,false*/);
            }
            else
            {
                if (PrevContains(newval))
                {
                    Reset(newval, newtime, Math.Abs((SumDelta + delta) / SumDelta) < 0.5f/*, false*/);
                    isEvent = false;
                }
                //kiem tra tốc độ đổ dầu nếu < 10 lít trong 10 phút thì đây là sự kiện giả
                else if (SumDelta > 0 && CounterSeconds > 0 && SumDelta / CounterSeconds < 16.6)
                {
                    Reset(newval, newtime, true/*, false*/);
                    isEvent = false;
                }
            }

            return isEvent;
        }

        public int CompareTo(FuelEventCandidate that)
        {
            if (that == null) return 1;
            if (this.FistTime > that.FistTime) return 1;
            if (this.FistTime < that.FistTime) return -1;
            return 0;
        }

        public void Deserializer(BinaryReader stream, int version)
        {
            FistTime = DateTime.FromBinary(stream.ReadInt64());
            FistValue = stream.ReadSingle();
            PrevTime = DateTime.FromBinary(stream.ReadInt64());
            PrevValue = stream.ReadSingle();
            SumDelta = stream.ReadSingle();
            Counter = stream.ReadInt16();
            NextCounter = stream.ReadByte();
            PrevNoise = stream.ReadSingle();
            Prevs.Clear(); int n = stream.ReadInt32();
            for (int i = 0; i < n; i++)
                Prevs.Add(stream.ReadSingle());

            if (version >= 13) CounterSeconds = stream.ReadInt32();

            if(version>=15)
            {
                MaxDelta = stream.ReadSingle();
                MaxDeltaTime = DateTime.FromBinary(stream.ReadInt64());
                MaxDeltaLat = stream.ReadSingle();
                MaxDeltaLng = stream.ReadSingle();
            }
        }

        public void Serializer(BinaryWriter stream)
        {
            stream.Write(FistTime.ToBinary());
            stream.Write(FistValue);
            stream.Write(PrevTime.ToBinary());
            stream.Write(PrevValue);
            stream.Write(SumDelta);
            stream.Write(Counter);
            stream.Write(NextCounter);
            stream.Write(PrevNoise);
            stream.Write(Prevs.Count);
            foreach (float item in Prevs)
                stream.Write(item);

            //version 13
            stream.Write(CounterSeconds);

            //version 15
            stream.Write(MaxDelta);
            stream.Write(MaxDeltaTime.ToBinary());
            stream.Write(MaxDeltaLat);
            stream.Write(MaxDeltaLng);
        }


    }


}
