using System;
using UnityEngine;

namespace DefaultNamespace
{
	[RequireComponent(typeof(PositionSaver))]
	public class ReplayMover : MonoBehaviour
	{
		private PositionSaver _save;

		private int _index;
		private PositionSaver.Data _prev;
		private float _duration;

		private void Start()
		{
			// todo comment: зачем нужны эти проверки?
			// эти проверки выводят сообщение об ошибке в случае если компонент будет не найден или даже если найден, 
			// то данных в нем нет, компонент выключается для безопасности 
			if (!TryGetComponent(out _save) || _save.Records.Count == 0)
			{
				Debug.LogError("Records incorrect value", this);
				// todo comment: Для чего выключается этот компонент?
				// при том, что условие верно, нужно отключить компонент, чтобы скрипт не работал некорректно
				enabled = false;
			}
		}

		private void Update()
		{
			var curr = _save.Records[_index];
			// todo comment: Что проверяет это условие (с какой целью)? 
			// если получив запись по индексу проверяем, что время в текущей записи (curr.Time) не больше времени
			// с моменты старта игры (Time.time). если время текущей записи уже прошло, сохраняем текущую запись
			// как предыдущую (_prev = curr) и переходим к следующей записи (_index++).
			if (Time.time > curr.Time)
			{
				_prev = curr;
				_index++;
				// todo comment: Для чего нужна эта проверка?
				// если все записи (_save.Records.Count) уже закончились (_index>=), выключаем воспроизведение
				// (enabled = false) и сообщаем об окончании в Debug.Log сообщении. таким образом все движения
				// объектов будут воспроизводиться в правильном темпе.
				if (_index >= _save.Records.Count)
				{
					enabled = false;
					Debug.Log($"<b>{name}</b> finished", this);
				}
			}
			// todo comment: Для чего производятся эти вычисления (как в дальнейшем они применяются)?
			// вычисляем дельту - нормализированное время, прошедшее между двумя записями. 
			// (urr.Time - _prev.Time) общая длительность перехода и (Time.time - _prev.Time) прошедшее время с момента
			// старта перехода между записями. без этого объект бы не плавно перемещался, а телепортировался между 
			// точками.
			var delta = (Time.time - _prev.Time) / (curr.Time - _prev.Time);
			// todo comment: Зачем нужна эта проверка?
			// защита от деления на 0, если дельта Not a Number, то устанавливаем дельту в предыдущей позиции _prev.
			if (float.IsNaN(delta)) delta = 0f;
			// todo comment: Опишите, что происходит в этой строчке так подробно, насколько это возможно
			// transform.position - это текущая позиция объекта. через функцию линейной интерполяции Lerp мы 
			// выполняем плавный переход delta между предыдущей позицией и нынешней позицией. 
			transform.position = Vector3.Lerp(_prev.Position, curr.Position, delta);
		}
	}
}