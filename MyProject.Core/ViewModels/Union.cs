namespace MyProject.Core.ViewModels
{
    /// <summary>
    /// 공용체 지원을 위한 타입
    /// </summary>
    public record Union<T1, T2>(string Type, object Data)
    {
        public static implicit operator Union<T1, T2>(T1 t1)
            => new Union<T1, T2>(typeof(T1).Name, t1);

        public static implicit operator Union<T1, T2>(T2 t2)
            => new Union<T1, T2>(typeof(T2).Name, t2);
    }

    /// <summary>
    /// 공용체 지원을 위한 타입
    /// </summary>
    public record Union<T1, T2, T3>(string Type, object Data)
    {
        public static implicit operator Union<T1, T2, T3>(T1 t1)
            => new Union<T1, T2, T3>(typeof(T1).Name, t1);

        public static implicit operator Union<T1, T2, T3>(T2 t2)
            => new Union<T1, T2, T3>(typeof(T2).Name, t2);

        public static implicit operator Union<T1, T2, T3>(T3 t3)
            => new Union<T1, T2, T3>(typeof(T3).Name, t3);
    }

    /// <summary>
    /// 공용체 지원을 위한 타입
    /// </summary>
    public record Union<T1, T2, T3, T4>(string Type, object Data)
    {
        public static implicit operator Union<T1, T2, T3, T4>(T1 t1)
            => new Union<T1, T2, T3, T4>(typeof(T1).Name, t1);

        public static implicit operator Union<T1, T2, T3, T4>(T2 t2)
            => new Union<T1, T2, T3, T4>(typeof(T2).Name, t2);

        public static implicit operator Union<T1, T2, T3, T4>(T3 t3)
            => new Union<T1, T2, T3, T4>(typeof(T3).Name, t3);

        public static implicit operator Union<T1, T2, T3, T4>(T4 t4)
            => new Union<T1, T2, T3, T4>(typeof(T4).Name, t4);
    }
}
