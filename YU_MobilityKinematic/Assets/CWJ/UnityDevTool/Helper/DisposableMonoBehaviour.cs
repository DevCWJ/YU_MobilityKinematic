using UnityEngine;

namespace CWJ
{

    /// <summary>
    ///<br/>  using (fileChangedChecker = gameObject.AddComponent(FileChangedChecker()))
    ///<br/>  {
    ///<br/>      fileChangedChecker.fileChangedEvent.AddListener_New(UpdateCommandByTxt);
    ///<br/>      fileChangedChecker.InitSystemWatcher(Path.GetDirectoryName(commandTxtPath), Path.GetFileName(commandTxtPath));
    ///<br/>  }
    ///<para>�̷��� ��������</para>
    /// </summary>
    public abstract class DisposableMonoBehaviour : MonoBehaviour, System.IDisposable
    {
        protected abstract void OnDispose();

        /// <summary>
        /// Dispose�� �ɶ� (using ����) Destroy�Ǳ� ���ϸ� true
        /// <br/> default : true
        /// </summary>
        protected virtual bool isAutoDestroy => true;
        /// <summary>
        /// OnDispose�� �ҷȰ�, ����ó���� �Ǿ�����? �ߺ����������.
        /// </summary>
        protected bool isDesposed { get; private set; } = false;
        /// <summary>
        /// ������Ʈ Destroy Ȥ�� �����ᰡ �Ǵ°� �̸��˶� �ҷ��ָ� ����.
        /// <br/>call�Ҷ� ��� �ؾ� �ߺ����� ������.
        /// </summary>
        public void Dispose()
        {
            if (isDesposed)
            {
                return;
            }
            isDesposed = true;
            OnDispose();
            if (isAutoDestroy && !isDestroyed && !MonoBehaviourEventHelper.IS_QUIT)
            {
                Destroy(this);
            }
        }

        protected bool isDestroyed { get; private set; } = false;
        protected virtual void _OnDestroy() { }
        protected virtual void OnDestroy()
        {
            isDestroyed = true;
            _OnDestroy();
            Dispose();
        }
        protected virtual void _OnApplicationQuit() { }
        protected virtual void OnApplicationQuit()
        {
            _OnApplicationQuit();
            Dispose();
        }
    }
}
