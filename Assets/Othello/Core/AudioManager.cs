using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace RangerRPG.Core {
	public class AudioManager : SingletonBehaviour<AudioManager> {
		// Audio players components.
		public AudioSource dialogSource;
		public AudioSource musicSource;

		// Random pitch adjustment range.
		public float lowPitchRange = .95f;
		public float highPitchRange = 1.05f;
		
		public int dialogCompleted = 0;
		private Coroutine _audioRoutine;
		private bool _playing = false;
		private bool _stopAudio = false;
		private UnityAction _onCompletedAction = () => { };

		public override void Awake() {
			base.Awake();
			if (Instance == null || Instance == this) {
				DontDestroyOnLoad(gameObject);	
			}
		}

		// Play a single clip through the sound effects source.
		public void Play(AudioClip clip) {
			if (clip == null) return;
			dialogSource.clip = clip;
			dialogSource.Play();
		}
		
		public void Play(List<AudioClip> clips, UnityAction onCompleted) {
			if (_playing) {
				StopCoroutine(_audioRoutine);
				_onCompletedAction();
			}
			_onCompletedAction = onCompleted;
			_audioRoutine = StartCoroutine(PlayAudioClips(clips));
		}

		private IEnumerator PlayAudioClips(List<AudioClip> clips) {
			_playing = true;
			foreach (var clip in clips) {
				if (_stopAudio) {
					_stopAudio = false;
					break;
				}
				Play(clip);
				yield return new WaitForSeconds(clip.length + 0.5f);
			}
			_playing = false;
			_onCompletedAction();
		}

		// Play a single clip through the music source.
		public void PlayMusic(AudioClip clip) {
			musicSource.clip = clip;
			musicSource.Play();
		}

		public void PlayMusic() {
			if (musicSource.clip) {
				musicSource.Play();
			}
		}

		// Play a random clip from an array, and randomize the pitch slightly.
		public void RandomSoundEffect(params AudioClip[] clips) {
			int randomIndex = Random.Range(0, clips.Length);
			float randomPitch = Random.Range(lowPitchRange, highPitchRange);

			dialogSource.pitch = randomPitch;
			dialogSource.clip = clips[randomIndex];
			dialogSource.Play();
		}

		private void Update() {
			if (Input.GetKeyDown(KeyCode.N)) {
				_stopAudio = true;
			}
		}

		public void StopDialog() {
			dialogSource.Stop();
		}
		public bool IsDialogPlaying()
		{
			if (dialogSource.isPlaying)
			{
				return true;
			}
			return false;
		}
	}
}