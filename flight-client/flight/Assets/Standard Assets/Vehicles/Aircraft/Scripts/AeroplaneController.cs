using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Aeroplane
{
    [RequireComponent(typeof (Rigidbody))]
    public class AeroplaneController : MonoBehaviour
    {
        [SerializeField] private float m_MaxEnginePower = 40f;        // エンジンの最大出力
        [SerializeField] private float m_Lift = 0.002f;               // 前方に移動する飛行機が生み出す揚力
        [SerializeField] private float m_ZeroLiftSpeed = 300;         // 揚力が適用されなくなる速度
        [SerializeField] private float m_RollEffect = 1f;             // ロールの入力に対してどれだけの効果を与えるか
        [SerializeField] private float m_PitchEffect = 0.5f;          // ピッチの入力に対してどれだけの効果を与えるか
        [SerializeField] private float m_YawEffect = 0.2f;            // ヨーの入力に対して、どれだけの効果を与えるか
        [SerializeField] private float m_BankedTurnEffect = 0.5f;     // バンクターンを行っているときのターンの量
        [SerializeField] private float m_AerodynamicEffect = 0.02f;   // 空力がどれくらい飛行機の速度に影響を与えるか
        [SerializeField] private float m_AutoTurnPitch = 0.5f;        // バンクターン中に、飛行機が自動的に行うピッチングの量
        [SerializeField] private float m_AutoRollLevel = 0.2f;        // ロールを行っていないとき、飛行機がどれくらい水平になろうとするか
        [SerializeField] private float m_AutoPitchLevel = 0.2f;       // ピッチを行っていないとき、飛行機がどれくらい水平になろうとするか
        [SerializeField] private float m_AirBrakesEffect = 3f;        // エアブレーキがどれだけの抗力を生み出すか
        [SerializeField] private float m_ThrottleChangeSpeed = 0.3f;  // スロットルが変化する速度
        [SerializeField] private float m_DragIncreaseFactor = 0.001f; // 速度に応じてどれぐらい抗力が上昇するか

        public float Altitude { get; private set; }                     // 飛行機の地上からの高さ 
        public float Throttle { get; private set; }                     // 使用されているスロットルの量
        public bool AirBrakes { get; private set; }                     // エアブレーキが適用されているかどうか
        public float ForwardSpeed { get; private set; }                 // 飛行機が前方に進む速度
        public float EnginePower { get; private set; }                  // エンジンに与えられる力
        public float MaxEnginePower{ get { return m_MaxEnginePower; }}    // エンジンの最大出力
        public float RollAngle { get; private set; }                      // ロールの角度
        public float PitchAngle { get; private set; }                     // ピッチの角度
        public float RollInput { get; private set; }                      // ロール入力の際に与えられる力 
        public float PitchInput { get; private set; }                     // ピッチ入力の際に与えられる力
        public float YawInput { get; private set; }                       //ヨー入力の際に与えられる力
        public float ThrottleInput { get; private set; }                  //スロットル入力の際に与えられる力

        private float m_OriginalDrag;         // シーンが開始された時のDrag(RigidBody)
        private float m_OriginalAngularDrag;  // シーンが開始された時のAngularDrag(RigidBody)
        private float m_AeroFactor;
        private bool m_Immobilized = false;   // used for making the plane uncontrollable, i.e. if it has been hit or crashed.飛行機が制御不能になったとき使用
        private float m_BankedTurnAmount;
        private Rigidbody m_Rigidbody;
	    WheelCollider[] m_WheelColliders;


        private void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            // Store original drag settings, these are modified during flight.(元のDragを取得する、飛行中にDragは変更される)
            m_OriginalDrag = m_Rigidbody.drag;
            m_OriginalAngularDrag = m_Rigidbody.angularDrag;

			for (int i = 0; i < transform.childCount; i++ )
			{
				foreach (var componentsInChild in transform.GetChild(i).GetComponentsInChildren<WheelCollider>())
				{
					componentsInChild.motorTorque = 0.18f;
				}
			}//飛行機の車輪用の抗力計算 モデルが設定されている場合は使用する。
        }


        public void Move(float rollInput, float pitchInput, float yawInput, float throttleInput, bool airBrakes)
        {
            // transfer input parameters into properties.s 入力パラメーターを転送
            RollInput = rollInput;
            PitchInput = pitchInput;
            YawInput = yawInput;
            ThrottleInput = throttleInput;
            AirBrakes = airBrakes;

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
            RollInput = Mathf.Clamp(RollInput, -1, 1);
            PitchInput = Mathf.Clamp(PitchInput, -1, 1);
            YawInput = Mathf.Clamp(YawInput, -1, 1);
            ThrottleInput = Mathf.Clamp(ThrottleInput, -1, 1);
        }


        private void CalculateRollAndPitchAngles()
        {
            //ロール、およびピッチの角度を計算する
            //水平な前に進もうとする力を計算する(y軸の方向性は考慮しない)
            // Calculate the flat forward direction (with no y component).
            var flatForward = transform.forward;
            flatForward.y = 0;
            // If the flat forward vector is non-zero (which would only happen if the plane was pointing exactly straight upwards)
            //平坦な前方ベクトルがゼロ以外の場合（平面が正確に真上を向いている場合にのみ発生します）
            if (flatForward.sqrMagnitude > 0)
            {
                flatForward.Normalize();//ベクトルを正規化する
                // ピッチの角度を計算
                var localFlatForward = transform.InverseTransformDirection(flatForward);//ワールド座標からローカル座標への変換
                PitchAngle = Mathf.Atan2(localFlatForward.y, localFlatForward.z);//localFlatForward.zを中心として、localFlatForward.yが何度の位置にあるか。
                // ロールの角度を計算
                var flatRight = Vector3.Cross(Vector3.up, flatForward);//外積を求めることで、右の角度か左の角度かを判断する
                var localFlatRight = transform.InverseTransformDirection(flatRight);
                RollAngle = Mathf.Atan2(localFlatRight.y, localFlatRight.x);
            }
        }


        private void AutoLevel()//自動的に角度を修正する
        {
            // The banked turn amount (between -1 and 1) is the sine of the roll angle.
            //-1から1の間で定義されるバンクターンの量は、ロールの角度の正弦(傾いてできた角度との正弦)
            //http://www.cfijapan.com/study/html/to199/html-to175/151a-turning.htm 参照
            // this is an amount applied to elevator input if the user is only using the banking controls,
            //これは、プレイヤーがバンキングコントロールのみを使用している場合に段階的な入力に適用される量
            // because that's what people expect to happen in games!
            m_BankedTurnAmount = Mathf.Sin(RollAngle);
            // auto level roll, if there's no roll input:
            // ロール入力がされていないとき、自動的に行われるロール
            if (RollInput == 0f)
            {
                RollInput = -RollAngle*m_AutoRollLevel;
            }
            // auto correct pitch, if no pitch input (but also apply the banked turn amount)
            if (PitchInput == 0f)
            {
                PitchInput = -PitchAngle*m_AutoPitchLevel;
                PitchInput -= Mathf.Abs(m_BankedTurnAmount*m_BankedTurnAmount*m_AutoTurnPitch);
            }
        }


        private void CalculateForwardSpeed()
        {
            //前進速度は、飛行機が前に進もうとしているときの速度であって、ストールしているときの速度とは別
            // Forward speed is the speed in the planes's forward direction (not the same as its velocity, eg if falling in a stall)
            var localVelocity = transform.InverseTransformDirection(m_Rigidbody.velocity);
            ForwardSpeed = Mathf.Max(0, localVelocity.z);
        }


        private void ControlThrottle()
        {
            // override throttle if immobilized
            // スロットルが固定されている場合は、オーバーライド(上書き)する
            if (m_Immobilized)
            {
                ThrottleInput = -0.5f;
            }

            // Adjust throttle based on throttle input (or immobilized state)
            //スロットルの入力に応じて(もしくは入力が固定されているとき)、スロットルを調整する 変数として定義済み
            Throttle = Mathf.Clamp01(Throttle + ThrottleInput*Time.deltaTime*m_ThrottleChangeSpeed);

            // current engine power is just:
            //現在のエンジン出力
            EnginePower = Throttle*m_MaxEnginePower;
        }


        private void CalculateDrag()
        {
            // increase the drag based on speed, since a constant drag doesn't seem "Real" (tm) enough
            //リアルに見せるために速度に応じて、抗力を上昇させるようにする
            float extraDrag = m_Rigidbody.velocity.magnitude*m_DragIncreaseFactor;
            // Air brakes work by directly modifying drag. This part is actually pretty realistic!
            // エアブレーキは抗力から直接取得する
            m_Rigidbody.drag = (AirBrakes ? (m_OriginalDrag + extraDrag)*m_AirBrakesEffect : m_OriginalDrag + extraDrag);
            // Forward speed affects angular drag - at high forward speed, it's much harder for the plane to spin
            //前進速度が速ければ速いほど、曲がりにくくなる
            m_Rigidbody.angularDrag = m_OriginalAngularDrag*ForwardSpeed;
        }


        private void CaluclateAerodynamicEffect()
        {
            // "Aerodynamic" calculations. This is a very simple approximation of the effect that a plane
            // 空力計算。これは、平面の効果の非常に単純な近似です
            // will naturally try to align itself in the direction that it's facing when moving at speed.
            //速度で移動するときに、自然と向き合う方向に自動的に整列しようとします。
            // Without this, the plane would behave a bit like the asteroids spaceship!
            //これなしだと、まるで宇宙船のような感じになるだろう。
            if (m_Rigidbody.velocity.magnitude > 0)
            {
                // compare the direction we're pointing with the direction we're moving:
                // 向いている方向と移動している方向を比較します。
                m_AeroFactor = Vector3.Dot(transform.forward, m_Rigidbody.velocity.normalized);
                // multipled by itself results in a desirable rolloff curve of the effect
                //乗算することで、空力の効果を高める
                m_AeroFactor *= m_AeroFactor;
                // Finally we calculate a new velocity by bending the current velocity direction towards
                // 最後に、現在の速度方向を曲げて新しい速度を計算します
                // the the direction the plane is facing, by an amount based on this aeroFactor
                // このaeroFactorに基づく量による、飛行機が向いている方向
                var newVelocity = Vector3.Lerp(m_Rigidbody.velocity, transform.forward*ForwardSpeed,
                                               m_AeroFactor*ForwardSpeed*m_AerodynamicEffect*Time.deltaTime);
                m_Rigidbody.velocity = newVelocity;
                //なめらかに移動させるために線形補完を行っている

                // also rotate the plane towards the direction of movement - this should be a very small effect, but means the plane ends up
                // 向いている方向に向けて、飛行機を移動させる
                // pointing downwards in a stall
                // ストールしているときは、下向きの力になる
                m_Rigidbody.rotation = Quaternion.Slerp(m_Rigidbody.rotation,
                                                      Quaternion.LookRotation(m_Rigidbody.velocity, transform.up),
                                                      m_AerodynamicEffect*Time.deltaTime);
            }
        }


        private void CalculateLinearForces()
        {
            // Now calculate forces acting on the aeroplane:
            // 飛行機に与える力を計算する
            // we accumulate forces into this variable:
            // この変数に力を与えていく
            var forces = Vector3.zero;
            // Add the engine power in the forward direction
            // 前進方向にエンジンの力
            forces += EnginePower*transform.forward;
            // The direction that the lift force is applied is at right angles to the plane's velocity (usually, this is 'up'!)
            //  揚力を、飛行機の速度に対して垂直に発生させる(通常、これは上に発生する) 
            var liftDirection = Vector3.Cross(m_Rigidbody.velocity, transform.right).normalized;
            // The amount of lift drops off as the plane increases speed - in reality this occurs as the pilot retracts the flaps
            // 飛行機の速度が上がると、揚力が低下する (パイロットがフラップをひっこめたときに発生する)
            // shortly after takeoff, giving the plane less drag, but less lift. Because we don't simulate flaps, this is a simple way of doing it automatically:
            //フラップを考慮しないため、離陸後に抗力が減ると同時に、揚力も減るようにする
            var zeroLiftFactor = Mathf.InverseLerp(m_ZeroLiftSpeed, 0, ForwardSpeed);
            // Calculate and add the lift power
            //揚力を計算し、加える。
            var liftPower = ForwardSpeed*ForwardSpeed*m_Lift*zeroLiftFactor*m_AeroFactor;
            forces += liftPower*liftDirection;
            // Apply the calculated forces to the the Rigidbody
            //計算した力を加える。
            m_Rigidbody.AddForce(forces);
        }


        private void CalculateTorque()//エンジン動力に抗するトルクを考慮する
        {
            // We accumulate torque forces into this variable:
            //変数にトルクの力を代入する
            var torque = Vector3.zero;
            // Add torque for the pitch based on the pitch input.
            torque += PitchInput*m_PitchEffect*transform.right;
            // Add torque for the yaw based on the yaw input.
            torque += YawInput*m_YawEffect*transform.up;
            // Add torque for the roll based on the roll input.
            torque += -RollInput*m_RollEffect*transform.forward;
            // Add torque for banked turning.
            torque += m_BankedTurnAmount*m_BankedTurnEffect*transform.up;
            // The total torque is multiplied by the forward speed, so the controls have more effect at high speed,
            // and little effect at low speed, or when not moving in the direction of the nose of the plane
            // (i.e. falling while stalled)
            m_Rigidbody.AddTorque(torque*ForwardSpeed*m_AeroFactor);
        }


        private void CalculateAltitude()
        {
            // Altitude calculations - we raycast downwards from the aeroplane
            // starting a safe distance below the plane to avoid colliding with any of the plane's own colliders
            var ray = new Ray(transform.position - Vector3.up*10, -Vector3.up);
            RaycastHit hit;
            Altitude = Physics.Raycast(ray, out hit) ? hit.distance + 10 : transform.position.y;
        }


        // Immobilize can be called from other objects, for example if this plane is hit by a weapon and should become uncontrollable
        public void Immobilize()
        {
            m_Immobilized = true;
        }


        // Reset is called via the ObjectResetter script, if present.
        public void Reset()
        {
            m_Immobilized = false;
        }
    }
}
