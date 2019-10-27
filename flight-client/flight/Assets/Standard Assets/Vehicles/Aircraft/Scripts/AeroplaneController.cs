using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Aeroplane
{
    [RequireComponent(typeof (Rigidbody))]
    public class AeroplaneController : MonoBehaviour
    {
        [SerializeField] private float m_MaxEnginePower = 40f;        // �G���W���̍ő�o��
        [SerializeField] private float m_Lift = 0.002f;               // �O���Ɉړ������s�@�����ݏo���g��
        [SerializeField] private float m_ZeroLiftSpeed = 300;         // �g�͂��K�p����Ȃ��Ȃ鑬�x
        [SerializeField] private float m_RollEffect = 1f;             // ���[���̓��͂ɑ΂��Ăǂꂾ���̌��ʂ�^���邩
        [SerializeField] private float m_PitchEffect = 0.5f;          // �s�b�`�̓��͂ɑ΂��Ăǂꂾ���̌��ʂ�^���邩
        [SerializeField] private float m_YawEffect = 0.2f;            // ���[�̓��͂ɑ΂��āA�ǂꂾ���̌��ʂ�^���邩
        [SerializeField] private float m_BankedTurnEffect = 0.5f;     // �o���N�^�[�����s���Ă���Ƃ��̃^�[���̗�
        [SerializeField] private float m_AerodynamicEffect = 0.02f;   // ��͂��ǂꂭ�炢��s�@�̑��x�ɉe����^���邩
        [SerializeField] private float m_AutoTurnPitch = 0.5f;        // �o���N�^�[�����ɁA��s�@�������I�ɍs���s�b�`���O�̗�
        [SerializeField] private float m_AutoRollLevel = 0.2f;        // ���[�����s���Ă��Ȃ��Ƃ��A��s�@���ǂꂭ�炢�����ɂȂ낤�Ƃ��邩
        [SerializeField] private float m_AutoPitchLevel = 0.2f;       // �s�b�`���s���Ă��Ȃ��Ƃ��A��s�@���ǂꂭ�炢�����ɂȂ낤�Ƃ��邩
        [SerializeField] private float m_AirBrakesEffect = 3f;        // �G�A�u���[�L���ǂꂾ���̍R�͂𐶂ݏo����
        [SerializeField] private float m_ThrottleChangeSpeed = 0.3f;  // �X���b�g�����ω����鑬�x
        [SerializeField] private float m_DragIncreaseFactor = 0.001f; // ���x�ɉ����Ăǂꂮ�炢�R�͂��㏸���邩

        public float Altitude { get; private set; }                     // ��s�@�̒n�ォ��̍��� 
        public float Throttle { get; private set; }                     // �g�p����Ă���X���b�g���̗�
        public bool AirBrakes { get; private set; }                     // �G�A�u���[�L���K�p����Ă��邩�ǂ���
        public float ForwardSpeed { get; private set; }                 // ��s�@���O���ɐi�ޑ��x
        public float EnginePower { get; private set; }                  // �G���W���ɗ^�������
        public float MaxEnginePower{ get { return m_MaxEnginePower; }}    // �G���W���̍ő�o��
        public float RollAngle { get; private set; }                      // ���[���̊p�x
        public float PitchAngle { get; private set; }                     // �s�b�`�̊p�x
        public float RollInput { get; private set; }                      // ���[�����͂̍ۂɗ^������� 
        public float PitchInput { get; private set; }                     // �s�b�`���͂̍ۂɗ^�������
        public float YawInput { get; private set; }                       //���[���͂̍ۂɗ^�������
        public float ThrottleInput { get; private set; }                  //�X���b�g�����͂̍ۂɗ^�������

        private float m_OriginalDrag;         // �V�[�����J�n���ꂽ����Drag(RigidBody)
        private float m_OriginalAngularDrag;  // �V�[�����J�n���ꂽ����AngularDrag(RigidBody)
        private float m_AeroFactor;
        private bool m_Immobilized = false;   // used for making the plane uncontrollable, i.e. if it has been hit or crashed.��s�@������s�\�ɂȂ����Ƃ��g�p
        private float m_BankedTurnAmount;
        private Rigidbody m_Rigidbody;
	    WheelCollider[] m_WheelColliders;


        private void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            // Store original drag settings, these are modified during flight.(����Drag���擾����A��s����Drag�͕ύX�����)
            m_OriginalDrag = m_Rigidbody.drag;
            m_OriginalAngularDrag = m_Rigidbody.angularDrag;

			for (int i = 0; i < transform.childCount; i++ )
			{
				foreach (var componentsInChild in transform.GetChild(i).GetComponentsInChildren<WheelCollider>())
				{
					componentsInChild.motorTorque = 0.18f;
				}
			}//��s�@�̎ԗ֗p�̍R�͌v�Z ���f�����ݒ肳��Ă���ꍇ�͎g�p����B
        }


        public void Move(float rollInput, float pitchInput, float yawInput, float throttleInput, bool airBrakes)
        {
            // transfer input parameters into properties.s ���̓p�����[�^�[��]��
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
            // ���͂�-1����1�ւ̊Ԃɐ�������
            RollInput = Mathf.Clamp(RollInput, -1, 1);
            PitchInput = Mathf.Clamp(PitchInput, -1, 1);
            YawInput = Mathf.Clamp(YawInput, -1, 1);
            ThrottleInput = Mathf.Clamp(ThrottleInput, -1, 1);
        }


        private void CalculateRollAndPitchAngles()
        {
            //���[���A����уs�b�`�̊p�x���v�Z����
            //�����ȑO�ɐi�����Ƃ���͂��v�Z����(y���̕������͍l�����Ȃ�)
            // Calculate the flat forward direction (with no y component).
            var flatForward = transform.forward;
            flatForward.y = 0;
            // If the flat forward vector is non-zero (which would only happen if the plane was pointing exactly straight upwards)
            //���R�ȑO���x�N�g�����[���ȊO�̏ꍇ�i���ʂ����m�ɐ^��������Ă���ꍇ�ɂ̂ݔ������܂��j
            if (flatForward.sqrMagnitude > 0)
            {
                flatForward.Normalize();//�x�N�g���𐳋K������
                // �s�b�`�̊p�x���v�Z
                var localFlatForward = transform.InverseTransformDirection(flatForward);//���[���h���W���烍�[�J�����W�ւ̕ϊ�
                PitchAngle = Mathf.Atan2(localFlatForward.y, localFlatForward.z);//localFlatForward.z�𒆐S�Ƃ��āAlocalFlatForward.y�����x�̈ʒu�ɂ��邩�B
                // ���[���̊p�x���v�Z
                var flatRight = Vector3.Cross(Vector3.up, flatForward);//�O�ς����߂邱�ƂŁA�E�̊p�x�����̊p�x���𔻒f����
                var localFlatRight = transform.InverseTransformDirection(flatRight);
                RollAngle = Mathf.Atan2(localFlatRight.y, localFlatRight.x);
            }
        }


        private void AutoLevel()//�����I�Ɋp�x���C������
        {
            // The banked turn amount (between -1 and 1) is the sine of the roll angle.
            //-1����1�̊ԂŒ�`�����o���N�^�[���̗ʂ́A���[���̊p�x�̐���(�X���Ăł����p�x�Ƃ̐���)
            //http://www.cfijapan.com/study/html/to199/html-to175/151a-turning.htm �Q��
            // this is an amount applied to elevator input if the user is only using the banking controls,
            //����́A�v���C���[���o���L���O�R���g���[���݂̂��g�p���Ă���ꍇ�ɒi�K�I�ȓ��͂ɓK�p������
            // because that's what people expect to happen in games!
            m_BankedTurnAmount = Mathf.Sin(RollAngle);
            // auto level roll, if there's no roll input:
            // ���[�����͂�����Ă��Ȃ��Ƃ��A�����I�ɍs���郍�[��
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
            //�O�i���x�́A��s�@���O�ɐi�����Ƃ��Ă���Ƃ��̑��x�ł����āA�X�g�[�����Ă���Ƃ��̑��x�Ƃ͕�
            // Forward speed is the speed in the planes's forward direction (not the same as its velocity, eg if falling in a stall)
            var localVelocity = transform.InverseTransformDirection(m_Rigidbody.velocity);
            ForwardSpeed = Mathf.Max(0, localVelocity.z);
        }


        private void ControlThrottle()
        {
            // override throttle if immobilized
            // �X���b�g�����Œ肳��Ă���ꍇ�́A�I�[�o�[���C�h(�㏑��)����
            if (m_Immobilized)
            {
                ThrottleInput = -0.5f;
            }

            // Adjust throttle based on throttle input (or immobilized state)
            //�X���b�g���̓��͂ɉ�����(�������͓��͂��Œ肳��Ă���Ƃ�)�A�X���b�g���𒲐����� �ϐ��Ƃ��Ē�`�ς�
            Throttle = Mathf.Clamp01(Throttle + ThrottleInput*Time.deltaTime*m_ThrottleChangeSpeed);

            // current engine power is just:
            //���݂̃G���W���o��
            EnginePower = Throttle*m_MaxEnginePower;
        }


        private void CalculateDrag()
        {
            // increase the drag based on speed, since a constant drag doesn't seem "Real" (tm) enough
            //���A���Ɍ����邽�߂ɑ��x�ɉ����āA�R�͂��㏸������悤�ɂ���
            float extraDrag = m_Rigidbody.velocity.magnitude*m_DragIncreaseFactor;
            // Air brakes work by directly modifying drag. This part is actually pretty realistic!
            // �G�A�u���[�L�͍R�͂��璼�ڎ擾����
            m_Rigidbody.drag = (AirBrakes ? (m_OriginalDrag + extraDrag)*m_AirBrakesEffect : m_OriginalDrag + extraDrag);
            // Forward speed affects angular drag - at high forward speed, it's much harder for the plane to spin
            //�O�i���x��������Α����قǁA�Ȃ���ɂ����Ȃ�
            m_Rigidbody.angularDrag = m_OriginalAngularDrag*ForwardSpeed;
        }


        private void CaluclateAerodynamicEffect()
        {
            // "Aerodynamic" calculations. This is a very simple approximation of the effect that a plane
            // ��͌v�Z�B����́A���ʂ̌��ʂ̔��ɒP���ȋߎ��ł�
            // will naturally try to align itself in the direction that it's facing when moving at speed.
            //���x�ňړ�����Ƃ��ɁA���R�ƌ������������Ɏ����I�ɐ��񂵂悤�Ƃ��܂��B
            // Without this, the plane would behave a bit like the asteroids spaceship!
            //����Ȃ����ƁA�܂�ŉF���D�̂悤�Ȋ����ɂȂ邾�낤�B
            if (m_Rigidbody.velocity.magnitude > 0)
            {
                // compare the direction we're pointing with the direction we're moving:
                // �����Ă�������ƈړ����Ă���������r���܂��B
                m_AeroFactor = Vector3.Dot(transform.forward, m_Rigidbody.velocity.normalized);
                // multipled by itself results in a desirable rolloff curve of the effect
                //��Z���邱�ƂŁA��͂̌��ʂ����߂�
                m_AeroFactor *= m_AeroFactor;
                // Finally we calculate a new velocity by bending the current velocity direction towards
                // �Ō�ɁA���݂̑��x�������Ȃ��ĐV�������x���v�Z���܂�
                // the the direction the plane is facing, by an amount based on this aeroFactor
                // ����aeroFactor�Ɋ�Â��ʂɂ��A��s�@�������Ă������
                var newVelocity = Vector3.Lerp(m_Rigidbody.velocity, transform.forward*ForwardSpeed,
                                               m_AeroFactor*ForwardSpeed*m_AerodynamicEffect*Time.deltaTime);
                m_Rigidbody.velocity = newVelocity;
                //�Ȃ߂炩�Ɉړ������邽�߂ɐ��`�⊮���s���Ă���

                // also rotate the plane towards the direction of movement - this should be a very small effect, but means the plane ends up
                // �����Ă�������Ɍ����āA��s�@���ړ�������
                // pointing downwards in a stall
                // �X�g�[�����Ă���Ƃ��́A�������̗͂ɂȂ�
                m_Rigidbody.rotation = Quaternion.Slerp(m_Rigidbody.rotation,
                                                      Quaternion.LookRotation(m_Rigidbody.velocity, transform.up),
                                                      m_AerodynamicEffect*Time.deltaTime);
            }
        }


        private void CalculateLinearForces()
        {
            // Now calculate forces acting on the aeroplane:
            // ��s�@�ɗ^����͂��v�Z����
            // we accumulate forces into this variable:
            // ���̕ϐ��ɗ͂�^���Ă���
            var forces = Vector3.zero;
            // Add the engine power in the forward direction
            // �O�i�����ɃG���W���̗�
            forces += EnginePower*transform.forward;
            // The direction that the lift force is applied is at right angles to the plane's velocity (usually, this is 'up'!)
            //  �g�͂��A��s�@�̑��x�ɑ΂��Đ����ɔ���������(�ʏ�A����͏�ɔ�������) 
            var liftDirection = Vector3.Cross(m_Rigidbody.velocity, transform.right).normalized;
            // The amount of lift drops off as the plane increases speed - in reality this occurs as the pilot retracts the flaps
            // ��s�@�̑��x���オ��ƁA�g�͂��ቺ���� (�p�C���b�g���t���b�v���Ђ����߂��Ƃ��ɔ�������)
            // shortly after takeoff, giving the plane less drag, but less lift. Because we don't simulate flaps, this is a simple way of doing it automatically:
            //�t���b�v���l�����Ȃ����߁A������ɍR�͂�����Ɠ����ɁA�g�͂�����悤�ɂ���
            var zeroLiftFactor = Mathf.InverseLerp(m_ZeroLiftSpeed, 0, ForwardSpeed);
            // Calculate and add the lift power
            //�g�͂��v�Z���A������B
            var liftPower = ForwardSpeed*ForwardSpeed*m_Lift*zeroLiftFactor*m_AeroFactor;
            forces += liftPower*liftDirection;
            // Apply the calculated forces to the the Rigidbody
            //�v�Z�����͂�������B
            m_Rigidbody.AddForce(forces);
        }


        private void CalculateTorque()//�G���W�����͂ɍR����g���N���l������
        {
            // We accumulate torque forces into this variable:
            //�ϐ��Ƀg���N�̗͂�������
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
