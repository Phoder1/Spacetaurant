//using System;
//using UnityEngine;
//using UnityEngine.Events;

//namespace Spacetaurant
//{
//    public class InputManager : MonoSingleton<InputManager>
//    {
//        public UnityEventForRefrence _interact;
//        private void Start()
//        {
//            InputMap.Move.OnStart += _ => Debug.Log("Start");
//            InputMap.Move.OnPerform += _ => Debug.Log("Performed");
//            InputMap.Move.OnStop += _ => Debug.Log("Stop");
//        }
//        // Update is called once per frame
//        void LateUpdate()
//        {
//            InputMap.Move
//        }
//    }
//    public static class InputMap
//    {
//        public static Input<Vector2> Move = new Input<Vector2>((X) => X != Vector2.zero);
//        public static Input<bool> Interact = new Input<bool>((X) => X != false);
//    }
//    public class Input<T>
//    {
//        public delegate bool ValueValid(T value);
//        private ValueValid _valueCheck = default;
//        private T _lastValue = default;
//        private T _value = default;

//        public bool _recievedInput;

//        public event Action<T> OnStart = default;
//        public event Action<T> OnPerform = default;
//        public event Action<T> OnStop = default;
//        public Input(ValueValid valueCheck)
//        {
//            _valueCheck = valueCheck;
//        }

//        public T Value
//        {
//            get => _value;
//            set
//            {
//                if ((_value == null && value == null) || (_value != null && _value.Equals(value)))
//                    return;
//                _lastValue = _value;
//                _value = value;

//                if (_valueCheck(_value))
//                    _recievedInput = true;
//            }
//        }
//        private void CheckStatus(T oldValue, T newValue)
//        {
//            bool wasValid = _valueCheck(oldValue);
//            bool isValid = _valueCheck(newValue);



//            if (!wasValid && isValid)
//                OnStart?.Invoke(_value);

//            if (isValid)
//                OnPerform?.Invoke(_value);

//            if (wasValid && !isValid)
//                OnStop?.Invoke(_value);
//        }


//    }
//}
