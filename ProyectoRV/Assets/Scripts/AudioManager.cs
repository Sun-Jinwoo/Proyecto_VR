using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instancia;

    [Header("Audio Sources")]
    public AudioSource efectosSource;
    public AudioSource musicaSource;

    [Header("Clips")]
    public AudioClip click;
    public AudioClip hover;
    public AudioClip musicaFondo;

    // entradas dinamicas para sonidos del juego

    [Serializable]
    public class AudioEntry
    {
        [Tooltip("Nombre clave para llamar este sonido desde otros scripts")]
        public string name;

        [Range(0f, 1f)]
        [Tooltip("Volumen de esta categoría")]
        public float volume = 1f;

        [Tooltip("Agrega todos los clips posibles — se elige uno aleatorio al reproducir")]
        public AudioClip[] clips;
    }

    [Header("Lista de Audios")]
    [Tooltip("Agrega una entrada por cada tipo de sonido del juego")]
    public List<AudioEntry> audioEntries = new List<AudioEntry>();

    [Header("Configuración Pool")]
    [SerializeField]
    [Tooltip("Cuántos sonidos pueden sonar al mismo tiempo")]
    private int audioSourcePoolSize = 5;

    private AudioSource[] audioSources;
    private int currentSource = 0;
    private Dictionary<string, AudioEntry> audioMap;

    public static AudioManager Instance => instancia;


    void Awake()
    {
        // Singleton original — conservado intacto
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        BuildAudioMap();
        CreateAudioSourcePool();
    }


    void Start()
    {
        // Música de fondo
        if (musicaFondo != null && !musicaSource.isPlaying)
        {
            musicaSource.clip = musicaFondo;
            musicaSource.loop = true;
            musicaSource.Play();
        }
    }

    // ?? Métodos originales

    public void PlayClick()
    {
        if (instancia != null && click != null)
            instancia.efectosSource.PlayOneShot(click);
    }

    public void PlayHover()
    {
        if (instancia != null && hover != null)
            instancia.efectosSource.PlayOneShot(hover);
    }

    public void PlaySound(AudioClip clip)
    {
        if (instancia != null && clip != null)
            instancia.efectosSource.PlayOneShot(clip);
    }

    // ?? Sistema de entradas dinámicas

    void BuildAudioMap()
    {
        audioMap = new Dictionary<string, AudioEntry>();
        foreach (var entry in audioEntries)
        {
            if (string.IsNullOrEmpty(entry.name)) continue;

            if (!audioMap.ContainsKey(entry.name))
                audioMap.Add(entry.name, entry);
            else
                Debug.LogWarning($"[AudioManager] Nombre duplicado: '{entry.name}' — solo se usará el primero");
        }
    }

    void CreateAudioSourcePool()
    {
        audioSources = new AudioSource[audioSourcePoolSize];
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            var go = new GameObject($"AudioSource_{i}");
            go.transform.SetParent(transform);
            audioSources[i] = go.AddComponent<AudioSource>();
        }
    }

    // Reproduce un clip aleatorio de la categoría
    public void Play(string entryName)
    {
        Debug.Log($"[AudioManager] Play: {entryName}");
        if (!audioMap.TryGetValue(entryName, out AudioEntry entry))
        {
            Debug.LogWarning($"[AudioManager] Entrada no encontrada: '{entryName}'");
            return;
        }

        if (entry.clips == null || entry.clips.Length == 0)
        {
            Debug.LogWarning($"[AudioManager] '{entryName}' no tiene clips asignados");
            return;
        }

        AudioClip clip = entry.clips[Random.Range(0, entry.clips.Length)];
        AudioSource source = audioSources[currentSource];
        currentSource = (currentSource + 1) % audioSources.Length;

        source.volume = entry.volume;
        source.PlayOneShot(clip);
    }
}