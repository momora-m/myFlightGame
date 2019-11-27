using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Fighter// 戦闘機周りはこの名前空間で統一
{
    [RequireComponent(typeof(Rigidbody))]//Rigidbodyが無い時強制的にアタッチする
    public class FighterController : MonoBehaviour
    {
        [SerializeField] private float maxEnginePowerThrottle = 40f;        // エンジンの最大出力
        [SerializeField] private float liftFighter = 0.0025f;               // 前方に移動する飛行機生み出す揚力
        [SerializeField] private float zeroLiftSpeed = 300;         // 揚力が適用されなくなる速度
        [SerializeField] private float stoleSpeed = 60;             // ストールが開始される速度
        [SerializeField] private float rollEffect = 1f;             // ロールの入力に対してどれだけの効果を与えるか
        [SerializeField] private float pitchEffect = 0.5f;          // ピッチの入力に対してどれだけの効果を与えるか
        [SerializeField] private float yawEffect = 0.05f;            // ヨーの入力に対して、どれだけの効果を与えるか
        [SerializeField] private float bankedTurnEffect = 0.5f;     // バンクターンを行っているときのターンの量
        [SerializeField] private float aeroDynamicEffect = 0.02f;   // 空力がどれくらい飛行機の速度に影響を与えるか
        [SerializeField] private float autoTurnPitch = 0.5f;        // バンクターン中に、飛行機が自動的に行うピッチングの量
        [SerializeField] private float autoRollLevel = 0.2f;        // ロールを行っていないとき、飛行機がどれくらい水平になろうとするか
        [SerializeField] private float autoPitchLevel = 0.2f;       // ピッチを行っていないとき、飛行機がどれくらい水平になろうとするか
        [SerializeField] private float airBrakesEffect = 3f;        // エアブレーキがどれだけの抗力を生み出すか
        [SerializeField] private float throttleChangeSpeed = 0.3f;   // スロットルが変化する速度
        [SerializeField] private float dragIncreaseFactor = 0.001f;  // 速度に応じてどれぐらい抗力が上昇するか
        [SerializeField] private float airDensity = 0.01f;　　     　//大気中の空気密度
        [SerializeField] private float constantLift = 0.5f;         //揚力計算の係数である1/2
        [SerializeField] private float wingSurface = 0.5f; 　　　　　//翼の面積
        //ここまでの値は、インスペクターで編集していじれる様にする


        public float altitude { get; private set; }                     // 飛行機の地上からの高さ 
        public float throttle { get; private set; }                     // 使用されているスロットルの量
        public bool airBrakes { get; private set; }                     // エアブレーキが適用されているかどうか
        public float forwardSpeed { get; private set; }                 // 飛行機が前方に進む速度
        public float enginePower { get; private set; }                  // エンジンに与えられる力
        public float maxEnginePower { get { return maxEnginePowerThrottle; } }    // エンジンの最大出力
        public float rollAngle { get; private set; }                      // ロールの角度
        public float pitchAngle { get; private set; }                     // ピッチの角度
        public float rollInput { get; private set; }                      // ロール入力の際に与えられる力 
        public float pitchInput { get; private set; }                     // ピッチ入力の際に与えられる力
        public float yawInput { get; private set; }                       //ヨー入力の際に与えられる力
        public float throttleInput { get; private set; }                  //スロットル入力の際に与えられる力
        public bool isAutoPilot { get; private set; }                     //オートパイロット状態か否か
        public bool isPitchup { get; private set; }

        private float originalDrag;         // 開始時点での空気抵抗
        private float originalAngularDrag;  // 開始時点での回転の抵抗
        private float aeroFactor;
        private bool immobilizedFighter = false;   //飛行機が制御不能(immobilized)になったとき使用
        private float bankedTurnAmount;
        private Rigidbody rigidbodyFighter;
        WheelCollider[] m_WheelColliders;

        // Start is called before the first frame update
        void Start()
        {
            rigidbodyFighter = GetComponent<Rigidbody>();
            //オブジェクト(戦闘機の)抗力の初期値を
            originalDrag = rigidbodyFighter.drag;
            originalAngularDrag = rigidbodyFighter.angularDrag;

            for (int i = 0; i < transform.childCount; i++)
            {
                foreach (var componentsInChild in transform.GetChild(i).GetComponentsInChildren<WheelCollider>())
                {
                    componentsInChild.motorTorque = 0.18f;
                }
            }//飛行機の車輪用の抗力計算 モデルが設定されている場合は使用する。
        }

        //飛行機の移動そのものに関する関数
        public void MoveFighter(float rollInputControll, float pitchInputControll, float yawInputControll, float throttleInputControll, bool airBrakesControll)
        {
            // 入力パラメーターを転送
            rollInput = rollInputControll;
            pitchInput = pitchInputControll;
            yawInput = yawInputControll;
            throttleInput = throttleInputControll;
            airBrakes = airBrakesControll;

            ClampInputs();

            CalculateRollAndPitchAngles();

            AutoLevel();

            CalculateForwardSpeed();

            ControlThrottle();

            CalculateDrag();

            CaluclateAerodynamicEffect();

            CalculateLinearForces();

            CalculateTorque();

            CalculateAltitude();

        }
        private void ClampInputs()
        {
            // 入力を-1から1への間に制限する
            rollInput = Mathf.Clamp(rollInput, -1f, 1f);
            pitchInput = Mathf.Clamp(pitchInput, -1f, 1f);
            yawInput = Mathf.Clamp(yawInput, -1, 1);
            throttleInput = Mathf.Clamp(throttleInput, -1, 1);
        }


        private void CalculateRollAndPitchAngles()//ロールとピッチの角度計算を行う
        {
            Vector3 flatForward = transform.forward;
            flatForward.y = 0;
            //前方ベクトルゼロ以外のとき
            if (flatForward.sqrMagnitude > 0)
            {
                flatForward.Normalize();//ベクトルを正規化する
                // ピッチの角度を計算
                Vector3 localFlatForward = transform.InverseTransformDirection(flatForward);//ワールド座標からローカル座標への変換
                pitchAngle = Mathf.Atan2(localFlatForward.y, localFlatForward.z);//ローカル座標を用いて仰角を計算する
                // ロールの角度を計算
                Vector3 flatRight = Vector3.Cross(Vector3.up, flatForward);//外積を求めることで、右の角度か左の角度かを判断する
                Vector3 localFlatRight = transform.InverseTransformDirection(flatRight);//ワールド座標からローカル座標への変換
                rollAngle = Mathf.Atan2(localFlatRight.y, localFlatRight.x);//ローカル座標を用いて方位角を計算する
            }
        }


        private void AutoLevel()//自動的に角度を修正する
        {
            //エスコン的操作のために、オートパイロット有効時に表示する。
            if (isAutoPilot == true)
            {
                //-1から1の間で定義されるバンクターンの量は、ロールの角度の正弦(傾いてできた角度との正弦)
                //http://www.cfijapan.com/study/html/to199/html-to175/151a-turning.htm 参照
                //これは、プレイヤーがバンキングコントロールのみを使用している場合に段階的な入力に適用される量

                // because that's what people expect to happen in games!
                bankedTurnAmount = Mathf.Sin(rollAngle);
                // ロール入力がされていないとき、自動的に行われるロール
                if (rollInput == 0f)
                {
                    rollInput = -rollAngle * autoRollLevel;
                }
                // auto correct pitch, if no pitch input (but also apply the banked turn amount)
                if (pitchInput == 0f)
                {
                    pitchInput = -pitchAngle * autoPitchLevel;
                    pitchInput -= Mathf.Abs(bankedTurnAmount * bankedTurnAmount * autoTurnPitch);
                }
            }
        }


        private void CalculateForwardSpeed()
        {
            //前進速度は、飛行機が前に進もうとしているときの速度であって、ストールしているときの速度とは別
            var localVelocity = transform.InverseTransformDirection(rigidbodyFighter.velocity);
            forwardSpeed = Mathf.Max(0, localVelocity.z);
        }


        private void ControlThrottle()
        {
            // override throttle if immobilized
            // スロットルが固定されている場合は、オーバーライド(上書き)する
            if (immobilizedFighter)
            {
                throttleInput = -0.5f;
            }

            // Adjust throttle based on throttle input (or immobilized state)
            //スロットルの入力に応じて(もしくは入力が固定されているとき)、スロットルを調整する 変数として定義済み
            throttle = Mathf.Clamp01(throttle + throttleInput * Time.deltaTime * throttleChangeSpeed);

            // current engine power is just:
            //現在のエンジン出力
            enginePower = throttle * maxEnginePowerThrottle;
        }


        private void CalculateDrag()
        {
            //速度が上がればあがるほど、翼にかかる誘導抗力は当然上昇するので、疑似的な係数を使って表現する。
            float extraDrag = rigidbodyFighter.velocity.magnitude * dragIncreaseFactor;
            // Air brakes work by directly modifying drag. This part is actually pretty realistic!
            // エアブレーキは抗力から直接取得する
            rigidbodyFighter.drag = (airBrakes ? (originalDrag + extraDrag) * airBrakesEffect : originalDrag + extraDrag);
            //前進速度が速ければ速いほど、曲がりにくくなる
            rigidbodyFighter.angularDrag = originalAngularDrag * forwardSpeed;
        }

        
        private void CaluclateAerodynamicEffect()
        {
            if (!isPitchup /*&& forwardSpeed > zeroLiftSpeed*/) {//ピッチアップしているときは、空力の影響を考慮しない
                //空力を考慮しないことにより、実にエスコン的な機動になる。
                // 空力計算を行う。これは、翼が生み出す翼平面の効果の非常に単純な近似です
                //速度で移動するときに、自然と向き合う方向に自動的に整列しようとします。
                //これをピッチアップ中は動作させないことで、現実ではありえないようなインメルマンターンを可能にする。
                if (rigidbodyFighter.velocity.magnitude > 0)//加速度がzeroより大きい時
                {
                    // 向いている方向と移動している方向(加速度から算出)を比較します。
                    aeroFactor = Vector3.Dot(transform.forward, rigidbodyFighter.velocity.normalized);
                    // multipled by itself results in a desirable rolloff curve of the effect
                    //乗算することで、空力の効果を高める
                    aeroFactor *= aeroFactor;
                    // 最後に、現在の速度方向を曲げて新しい速度を計算します
                    // このaeroFactorに基づく量による、飛行機が向いている方向に速度計算を行う+
                    var newVelocity = Vector3.Lerp(rigidbodyFighter.velocity, transform.forward * forwardSpeed,
                                                   aeroFactor * forwardSpeed * aeroDynamicEffect * Time.deltaTime);
                    rigidbodyFighter.velocity = newVelocity;
                    //なめらかに移動させるために線形補完を行っている

                    // also rotate the plane towards the direction of moveFighterment - this should be a very small effect, but means the plane ends up
                    // 向いている方向に向けて、飛行機を移動させる
                    // ストールしているときは、下向きの力になる 揚力を失う速度を考慮する
                    rigidbodyFighter.rotation = Quaternion.Slerp(rigidbodyFighter.rotation,
                                                          Quaternion.LookRotation(rigidbodyFighter.velocity, transform.up),
                                                          aeroDynamicEffect * Time.deltaTime);
                }
            }
        }


        private void CalculateLinearForces()
        {
            // 飛行機に与える力を計算する
            // この変数に力を与えていく
            var forces = Vector3.zero;
            // 前進方向にエンジンの力
            forces += enginePower * transform.forward;
            //  揚力を、飛行機の速度に対して垂直に発生させる 通常、前進時の加速度と、物体に対するX軸の方向は垂直なので、外積を求める
            //  必要なのは向き成分であり、外積の値は必要以上に大きくなりがちなので、正規化してしまう。
            
            var liftDirection = Vector3.Cross(rigidbodyFighter.velocity, transform.right).normalized;
            // 航空機は離陸すると、フラップを上げて揚力を必要以上に大きくしないようにする
            //フラップの上げ下げを考慮しないため、離陸後に抗力が減ると同時に、揚力も減るようにする
            var zeroLiftFactor = Mathf.InverseLerp(zeroLiftSpeed, 0, forwardSpeed);
            //揚力を計算し、加える。
            float liftConstantValue = wingSurface * constantLift * airDensity;
            float liftPower = forwardSpeed * forwardSpeed * liftConstantValue * zeroLiftFactor * aeroFactor;
            forces += liftPower * liftDirection;
            //計算した力を加える。
            rigidbodyFighter.AddForce(forces);
        }


        private void CalculateTorque()//エンジン動力に抗するトルクを考慮する このトルクこそが航空機の本質である。
        {
            //変数にトルクの力を代入する
            Vector3 torque = Vector3.zero;
            //ピッチの入力に基づいた、トルクを代入する
            torque += pitchInput * pitchEffect * transform.right;
            // ヨー
            torque += yawInput * yawEffect * transform.up;
            // ロール
            torque += -rollInput * rollEffect * transform.forward;
            // バンクターン
            torque += bankedTurnAmount * bankedTurnEffect * transform.up;
            //合計トルクに前進速度が乗算されるため、コントロールは高速でより効果があり、
            //低速で、または飛行機の機首の方向に移動していない場合はほとんど効果がありません
            //ストール中は落下する
            rigidbodyFighter.AddTorque(torque * forwardSpeed * aeroFactor);
        }


        private void CalculateAltitude()
        {
            // 飛行機の高度を計算する
            //飛行機自身のコライダーに衝突する可能性があるので、大体10ぐらい下から始める
            var ray = new Ray(transform.position - Vector3.up * 10, -Vector3.up);
            RaycastHit hit;
            altitude = Physics.Raycast(ray, out hit) ? hit.distance + 10 : transform.position.y;
            //地面があれば、それを10足したものが高度になる なかったら現状のY軸の位置をそのまま高度にする
        }


        // 建物とか武器とかにあたって制御不能という状態にしたいとき、これは外部から呼び出せる
        //敵の戦闘機とかに使えばそれっぽく墜落するので嬉しいかも
        public void Immobilize()
        {
            immobilizedFighter = true;
        }

        // ObjectResetter scriptを使って、これを呼び出す。
        public void Reset()
        {
            immobilizedFighter = false;
        }

        //主に入力による挙動制御のための関数 将来的に様々な挙動の考慮したいから、こんな感じの実装
        //WIP スマートじゃないので書き直す
        public void SetFighterStatus(bool isAutoPilotControll, bool isPitchupControll)
        {
            SetAutoPilot(isAutoPilotControll);

            SetPitchupStatus(isPitchupControll);
        }

        private void SetAutoPilot(bool isAutoPilotControll)
        {
            isAutoPilot = isAutoPilotControll;
        }

        private void SetPitchupStatus(bool isPitchupControll)
        {
            isPitchup = isPitchupControll;
        }
    }
}
