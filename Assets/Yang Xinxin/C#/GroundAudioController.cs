using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GroundAudioController : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("����������")]
    public float maxHearingDistance = 10f;
    [Tooltip("��С����")]
    [Range(0, 1)] public float minVolume = 0.1f;
    [Tooltip("�������")]
    [Range(0, 1)] public float maxVolume = 1f;
    [Tooltip("��������ƽ����")]
    [Range(1, 10)] public float volumeSmoothing = 5f;

    private AudioSource audioSource;
    private Transform player;
    private float targetVolume;

    void Awake()
    {
        // ��ȫ��ȡAudioSource���
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource���ȱʧ��", this);
            enabled = false;
            return;
        }

        // ��ʼ����Ƶ����
        audioSource.loop = true;
        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;
    }

    void Start()
    {
        // ����ȫ����Ҳ��ҷ�ʽ
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("δ�ҵ���Ҷ�����ȷ���������б�ǩΪ'Player'�Ķ���", this);
            enabled = false;
            return;
        }
        player = playerObj.transform;

        // ȷ����Ƶ����
        if (!audioSource.isPlaying && audioSource.clip != null)
        {
            audioSource.Play();
        }
        else if (audioSource.clip == null)
        {
            Debug.LogWarning("δ������Ƶ����", this);
        }
    }

    void Update()
    {
        // ˫�ؿ�ֵ���
        if (player == null || audioSource == null)
        {
            enabled = false;
            return;
        }

        // ������������
        float distance = Vector3.Distance(transform.position, player.position);
        targetVolume = distance <= maxHearingDistance ?
            Mathf.Lerp(minVolume, maxVolume, 1 - (distance / maxHearingDistance)) :
            0f;

        // ƽ����������
        audioSource.volume = Mathf.Lerp(
            audioSource.volume,
            targetVolume,
            Time.deltaTime * volumeSmoothing
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(transform.position, maxHearingDistance);
    }
}
