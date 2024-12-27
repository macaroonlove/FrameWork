using FrameWork.Editor;
using UnityEngine;

namespace Temporary.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/VFX", fileName = "VFX_", order = 1)]
    public class VFX : FX
    {
        [SerializeField, Label("파티클인가?")] private bool _isParticle;
        [SerializeField, Label("VFX")] private GameObject _vfxObj;
        [SerializeField, Label("생성 위치")] private ESpawnPoint _spawnPoint;
        [SerializeField, Label("지속 시간")] private float _duration;
        [SerializeField, Label("위치 오프셋")] private Vector3 _posOffset;
        [SerializeField, Label("회전 오프셋")] private Vector3 _rotOffset;

        [Space]
        [Tooltip("타겟 방식으로 보내야만 사용 가능")]
        [SerializeField, Label("적을 따라갈지 여부")] private bool _isFollowTarget;

        public override void Play(Unit target)
        {
            Vector3 pos = GetSpawnPoint(target);

            Quaternion baseRot = target.transform.rotation;
            Quaternion rot = baseRot * Quaternion.Euler(_rotOffset);

            var obj = Play(pos, rot);

            if (_isFollowTarget)
            {
                Follow follow = obj.gameObject.GetComponent<Follow>();
                if (follow == null)
                {
                    follow = obj.gameObject.AddComponent<Follow>();
                }
                follow.SetTarget(target.transform, _posOffset);
            }
        }

        public override void Play(Vector3 pos)
        {
            Quaternion rot = Quaternion.Euler(_rotOffset);

            Play(pos, rot);
        }

        private GameObject Play(Vector3 pos, Quaternion rot)
        {
            var poolSystem = BattleManager.Instance.GetSubSystem<PoolSystem>();
            var obj = poolSystem.Spawn(_vfxObj, _duration);

            pos += _posOffset;

            obj.transform.SetPositionAndRotation(pos, rot);

            if (_isParticle)
            {
                var particle = obj.GetComponent<ParticleSystem>();
                if (particle != null)
                {
                    particle.Play();
                }
            }

            return obj;
        }

        private Vector3 GetSpawnPoint(Unit target)
        {
            Vector3 point = target.transform.position;

            switch (_spawnPoint)
            {
                case ESpawnPoint.Head:
                    if (target.headPoint != null)
                    {
                        point = target.headPoint.position;
                    }
                    break;
                case ESpawnPoint.Body:
                    if (target.bodyPoint != null)
                    {
                        point = target.bodyPoint.position;
                    }
                    break;
                case ESpawnPoint.LeftHand:
                    if (target.leftHandPoint != null)
                    {
                        point = target.leftHandPoint.position;
                    }
                    break;
                case ESpawnPoint.RightHand:
                    if (target.rightHandPoint != null)
                    {
                        point = target.rightHandPoint.position;
                    }
                    break;
                case ESpawnPoint.Foot:
                    if (target.footPoint != null)
                    {
                        point = target.footPoint.position;
                    }
                    break;
                case ESpawnPoint.ProjectileHit:
                    if (target.projectileHitPoint != null)
                    {
                        point = target.projectileHitPoint.position;
                    }
                    break;
            }

            return point;
        }
    }
}