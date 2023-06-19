using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    internal class Container
    {
        private Dictionary<Type, Type> _dependencies;
        private Dictionary<Type, object> _singletone;
        private Dictionary<Type, bool> _isSingleton;

        public Container()
        {
            _dependencies = new Dictionary<Type, Type>();
            _singletone = new Dictionary<Type, object>();
            _isSingleton = new Dictionary<Type, bool>();
        }

        private void AddToDict<abstraction, implementation>(bool isSingleton)
        {
            Type key = typeof(abstraction);
            Type val = typeof(implementation);

            _dependencies[key] = val;

            if (!isSingleton) return;
            _isSingleton[key] = true;
        }

        public void AddSingleTon<abstraction, implementation>()
        {
            AddToDict<abstraction, implementation>(true);
        }

        public void AddTransient<abstraciton, implementation>()
        {
            AddToDict<abstraciton, implementation>(false);
        }

        public void Register<abstraction, implementation>() 
        {
            Type key = typeof(abstraction);
            Type val = typeof(implementation);

            _dependencies[key] = val;
        }

        public T Resolve<T>()
        {
            Type dependency = typeof(T);
            if (!_dependencies.ContainsKey(dependency)) throw new InvalidOperationException();
            if (_isSingleton[dependency] && _singletone[dependency] != null) return (T)_singletone[dependency];

            Type ob = _dependencies[dependency];
            ConstructorInfo ci = ob.GetConstructors()[0]; //vtvlit rom 1 constructor aris

            ParameterInfo[] pars = ci.GetParameters();
            object[] args = new object[pars.Length];
            for (int i = 0; i < pars.Length; i++)
            {
                ParameterInfo pi = pars[i];
                Type parType = pi.ParameterType;
                MethodInfo met = typeof(Container).GetMethod("Resolve").MakeGenericMethod(parType);
                args[i] = met.Invoke(null, new object[] { });
            }

            object res = ci.Invoke(args);
            if (_isSingleton[dependency])
            {
                _singletone[dependency] = res;
            }
            return (T)res;
        }  
    }
}
