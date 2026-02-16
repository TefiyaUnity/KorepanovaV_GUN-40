using UnityEngine;

namespace DefaultNamespace
{
	
	[RequireComponent(typeof(PositionSaver))]
	public class EditorMover : MonoBehaviour
	{
		private PositionSaver _save;
		private float _currentDelay;
		
		// todo comment: Что произойдёт, если _delay > _duration?
		// если _delay > _duration, записи будут меняться реже, чем нужно, воспроизведение будет медленным.

		[SerializeField, Range(0.2f, 1.0f)] 
		private float _delay = 0.5f;

		[SerializeField, Min(0.2f)] 
		private float _duration = 5f;

		private void Start()
		{
			// todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
			// Start() вызывается один раз, а Update() - каждый кадр. GetComponent тяжелая операция, чтобы воспроизводить 
			// ее каждый кадр. инициализируем компонент на старте, в Update прописываем логику
			if (_duration <= _delay)
			{
				_duration = _delay * 5f;
			}
			_save = GetComponent<PositionSaver>();
			_save.Records.Clear();
		}

		private void Update()
		{
			_duration -= Time.deltaTime;
			if (_duration <= 0f)
			{
				enabled = false;
				Debug.Log($"<b>{name}</b> finished", this);
				return;
			}
			
			// todo comment: Почему не написать (_delay -= Time.deltaTime;) по аналогии с полем _duration?
			// _duration - это общее время работы, которое уменьшается. _currentDelay отсчитывает время до следующей записи.
			// _currentDelay сбрасывается в _delay, и не должен уменьшаться совсем.
			_currentDelay -= Time.deltaTime;
			if (_currentDelay <= 0f)
			{
				_currentDelay = _delay;
				_save.Records.Add(new PositionSaver.Data
				{
					Position = transform.position,
					// todo comment: Для чего сохраняется значение игрового времени?
					// схранения позволяет системе знать где и когда был объект. это позволяет вычислить delta между точками
					// и воспроизводить движение в правильном темпе без сохранения времени записи воспроизводились бы 
					// мгновенно.
					Time = Time.time,
				});
			}
		}
	}
}