using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GroundAudioController : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("最大可听距离")]
    public float maxHearingDistance = 10f;
    [Tooltip("最小音量")]
    [Range(0, 1)] public float minVolume = 0.1f;
    [Tooltip("最大音量")]
    [Range(0, 1)] public float maxVolume = 1f;
    [Tooltip("音量过渡平滑度")]
    [Range(1, 10)] public float volumeSmoothing = 5f;

    private AudioSource audioSource;
    private Transform player;
    private float targetVolume;

    void Awake()
    {
        // 安全获取AudioSource组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource组件缺失！", this);
            enabled = false;
            return;
        }

        // 初始化音频设置
        audioSource.loop = true;
        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;
    }

    void Start()
    {
        // 更安全的玩家查找方式
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("未找到玩家对象，请确保场景中有标签为'Player'的对象", this);
            enabled = false;
            return;
        }
        player = playerObj.transform;

        // 确保音频播放
        if (!audioSource.isPlaying && audioSource.clip != null)
        {
            audioSource.Play();
        }
        else if (audioSource.clip == null)
        {
            Debug.LogWarning("未分配音频剪辑", this);
        }
    }

    void Update()
    {
        // 双重空值检查
        if (player == null || audioSource == null)
        {
            enabled = false;
            return;
        }

        // 计算距离和音量
        float distance = Vector3.Distance(transform.position, player.position);
        targetVolume = distance <= maxHearingDistance ?
            Mathf.Lerp(minVolume, maxVolume, 1 - (distance / maxHearingDistance)) :
            0f;

        // 平滑音量过渡
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
