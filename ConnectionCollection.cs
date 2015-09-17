using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirkServer
{
    class ConnectionCollection: ICollection<ConnectionModel>
    {
        private ConnectionModel[] connections;
        private bool _readOnly;
        private bool _isSynchronized;
        
        public int Count
        {
            get { return connections.Length; }
        }

        public bool IsReadOnly
        {
            get { return _readOnly; }
        }
        bool IsSynchronized
        {
            get
            {
                return _isSynchronized;
            }
        }

        public ConnectionCollection()
        {
            
        }

        private void Connect(ConnectionModel connection)
        {
                
        }

        private void Disconnect(ConnectionModel connection)
        {
            
        }

        private void Find(ConnectionModel connection)
        {
            
        }

        private void Broadcast(string Message)
        {
            
        }

        public IEnumerator<ConnectionModel> GetEnumerator()
        {
            return new List<ConnectionModel>.Enumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ConnectionModel item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(ConnectionModel item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(ConnectionModel[] conArray, int arrayIndex)
        {
            foreach (ConnectionModel con in conArray)
            {
                conArray.SetValue(con, arrayIndex);
                arrayIndex = arrayIndex + 1;
            }
        }

        public bool Remove(ConnectionModel item)
        {
            throw new NotImplementedException();
        }

    }
}
