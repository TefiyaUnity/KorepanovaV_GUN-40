using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
	public class PositionSaver : MonoBehaviour
	{
		public class ReadOnlyAttribute : PropertyAttribute { }
		
		[System.Serializable] 
		public struct Data
		{
			public Vector3 Position;
			public float Time;
		}

		[ReadOnly]
		[SerializeField]
		[Tooltip("To fill this field you need to use context menu in Inspector and the \"Create File\" command")]
		private TextAsset _json;

		 [ReadOnly]
		 [HideInInspector]
		 [SerializeField]
		 private List<Data> _records;
		public List<Data> Records 
    	{ 
        get => _records; 
        private set => _records = value; 
    	}

		private void Awake()
		{
			// todo comment: Что будет, если в теле этого условия не сделать выход из метода?
			// при отсутствии return в условии ниже, программа попытается выполнить JsonUtility.FromJsonOverwrite,
			// но т.к. _json равен null, обращение к _json.text вызовет ошибку, ссылка на объект будет null,
			// десериализация _json не получится.  
			if (_json == null)
			{
				//gameObject.SetActive(false);
				Debug.LogError("Please, create TextAsset and add in field _json");
				return;
			}
			
			JsonUtility.FromJsonOverwrite(_json.text, this);
			// todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
			// если в _json.text нет поля Records, метод FromJsonOverwrite помогает избежать ошибки
			// nullReference при дальнейшей работе. список с екомстью в 10 эл-тов создается при выполнении этого 
			// условия для дальнейшей работы. 
			if (_records == null)
				_records = new List<Data>(10);
		}

		private void OnDrawGizmos()
		{
			// todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
			// проверки в условии гарантируют, что последующая отрисовка Gizmos будет происходить только при наличии
			// данных в Records. это позволяет избежать появление ошибок по причине неинициализированного списка
			// и ошибок пустого списка.
			if (_records == null || _records.Count == 0) return;
			var data = _records;
			var prev = data[0].Position;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(prev, 0.3f);
			// todo comment: Почему итерация начинается не с нулевого элемента?
			// потому что нулевой элемент уже обработан до цикла (data[0] и линия чертится с позиции 0.3f.) линии
			// рисуются по последовательным точкам, от prev (первая точка), затем от curr(вторая) и далее по итерации. 
			for (int i = 1; i < data.Count; i++)
			{
				var curr = data[i].Position;
				Gizmos.DrawWireSphere(curr, 0.3f);
				Gizmos.DrawLine(prev, curr);
				prev = curr;
			}
		}
		
#if UNITY_EDITOR
		[ContextMenu("Create File")]
		private void CreateFile()
		{
			// todo comment: Что происходит в этой строке?
			// метод создает либо перезаписывает файл в пути Application.dataPath, возвращая поток для записи
			// данных в файл.
			var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
			// todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав)
			// строка нужна для последующего после создания пустого файла закрытия этого самого файла
			// чтобы тот появился в Assets папке. после AssetDatabase.Refresh() юнити увидит этот файл и создаст
			//  TextAsset. 
			stream.Dispose();
			UnityEditor.AssetDatabase.Refresh();
			//В Unity можно искать объекты по их типу, для этого используется префикс "t:"
			//После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)
			var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
			foreach (var guid in guids)
			{
				//Этой командой можно получить путь к ассету через его гуид
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				//Этой командой можно загрузить сам ассет
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
				//todo comment: Для чего нужны эти проверки?
				// если ассеты не равны null, и если в имени ассета присутствует "Path", ассет присваивается к _json
				// в данном случае прежде был создан "Path.txt", он не равен null, следовательно asset присвоится к 
				// полю _json (ссылке на текстовый ассет).
				if(asset != null && asset.name == "Path")
				{
					_json = asset;
					UnityEditor.EditorUtility.SetDirty(this);
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
					// todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
					// так как мы уже нашли TextAsset и присвоили его в _json = asset, а затем сказали Unity, 
					// что произошли изменения, сохранились и обновились, последующий поиск не нужен, 
					// т.к. процедура прошла успешно.
					return;
				}
			}
		}

		private void OnDestroy()
		{
	#if UNITY_EDITOR
    			if (_json != null && _records != null && _records.Count > 0)
        		{
            		string json = JsonUtility.ToJson(this, true);
            
            		string path = UnityEditor.AssetDatabase.GetAssetPath(_json);
            		string fullPath = Application.dataPath.Replace("Assets", "") + path;
            
            		File.WriteAllText(fullPath, json);
            
            		UnityEditor.AssetDatabase.Refresh();
            		Debug.Log($"Saved {_records.Count} records to {_json.name}");
        		}
	#endif
		}	
#endif
	}
}