#nullable enable

namespace MyProject.Core.ViewModels
{
    /// <summary>
    /// Detail 뷰모델을 따로 가질 때, 부모 또는 자식 클래스가 올 수 있음을 나타내는 타입
    /// </summary>
    /// <typeparam name="TBase">부모 클래스</typeparam>
    /// <typeparam name="TDerived"><c>TBase</c>를 상속한 Detail 뷰모델</typeparam>
    public record MaybeDetail<TBase, TDerived>(bool Detail, TBase Data)
        where TDerived : TBase
    {
        public static implicit operator MaybeDetail<TBase, TDerived>(TBase @base)
            => new MaybeDetail<TBase, TDerived>(@base is TDerived, @base);

        public static implicit operator MaybeDetail<TBase, TDerived>(TDerived derived)
            => new MaybeDetail<TBase, TDerived>(true, derived);
    }
}
