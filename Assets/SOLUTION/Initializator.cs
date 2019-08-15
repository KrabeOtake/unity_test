using UnityEngine;

namespace Assets.SOLUTION
{
    public class Initializator : MonoBehaviour
    {

      
        /// <summary>
        /// Ссылка на объект, чтобы его не удалил мусорщик
        /// </summary>
        private static Initializator Instance
        {
            get
            {
                if (_applicationIsQuitting) return null;

                if (_instance != null) return _instance;
                _instance = FindObjectOfType<Initializator>(); //чтобы не плодить объекты

                if (_instance != null) return _instance;
                _instance = new GameObject().AddComponent<Initializator>();
                _instance.gameObject.name = _instance.GetType().Name;
                return _instance;
            }

            set
            {
                if(_instance != null) return;
                _instance = value;
                DontDestroyOnLoad(_instance);
            }
        }

        private static Initializator _instance;

        private static bool _applicationIsQuitting;

        private bool IsInitialized { get; set; }

        /// <summary>
        /// Инициализация главных классов игры
        /// </summary>
        private void Awake()
        {
        
                if (IsInitialized || _instance != null) return;
                _instance = this;
                DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Инциализация сервисов после прогрузки всех объектов
        /// </summary>
        private void Start()
        {
            if (!IsInitialized || _instance != null) Initialize();
        }

        /// <summary>
        /// Загрузка 
        /// </summary>
        private void Initialize()
        {
            if (Instance.IsInitialized) return;
           
            IsInitialized = true;
        }

        /// <summary>
        /// При закрытии приложения
        /// </summary>
        private void OnApplicationQuit()
        {
            _instance = null;
            _applicationIsQuitting = true;
        }

        /// <summary>
        /// При нажатии паузы
        /// </summary>
        /// <param name="pause">состояние паузы</param>
        private void OnApplicationPause(bool pause)
        {

        }


        private void OnDestroy()
        {
            _instance = null;
            _applicationIsQuitting = true;
        }


    }
}
