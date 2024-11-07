using FrameWork.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    /// <summary>
    /// �� ���̺�(��������)�� ����
    /// </summary>
    [CreateAssetMenu(menuName = "Templates/Wave", fileName = "Wave", order = 0)]
    public class WaveTemplate : ScriptableObject
    {
        [Label("���� ���� �ð�")] public float spawnTime;

        public List<WaveInfo> waveInfo = new List<WaveInfo>();
    }

    [Serializable]
    public class WaveInfo
    {
        [Label("�� ����")] public EnemyTemplate template;

        [Label("���� ��ġ")] public Vector3 spawnPosition;
        [Tooltip("���� ���� �ð��� �������� �� �� �ִٰ� ������ ������")]
        [Label("������")] public float delayTime;
        [Label("������ ������ ��")] public int spawnCount = 1;
        [Label("���� ����")] public float spawnInterval = 0;

        [Label("���")] public List<Vector3> wayPoint = new List<Vector3>();
    }
}
