namespace WPF.Airprint.Mvvm
{
    using Prism.Mvvm;
    using Prism.Navigation;

    public abstract class ViewModelBase : BindableBase, IDestructible
    {
        protected ViewModelBase()
        {

        }

        public virtual void Destroy()
        {

        }
    }
}
