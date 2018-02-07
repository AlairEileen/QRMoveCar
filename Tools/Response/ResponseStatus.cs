
namespace Tools.Response
{
    public enum ResponseStatus
    {
        /// <summary>
        /// 请求成功
        /// </summary>
        请求成功 =1000,
        /// <summary>
        /// 请求失败
        /// </summary>
        请求失败 =2000,
        /// <summary>
        /// 请求参数不正确
        /// </summary>
        请求参数不正确 = 2001,
        /// <summary>
        /// 数据为空
        /// </summary>
        数据为空 =3000,
        /// <summary>
        /// 验证失败
        /// </summary>
        验证失败 =3001,
        /// <summary>
        /// 重复异常
        /// </summary>
        重复异常 =3002,
        /// <summary>
        /// 余额不足
        /// </summary>
        余额不足 =4000,
        /// <summary>
        /// 该账户已经领取
        /// </summary>
        该账户已经领取 =5000,
        /// <summary>
        /// 红包已经领取完成
        /// </summary>
        红包已经领取完成 =5001,
        /// <summary>
        /// 领取人数太多
        /// </summary>
        领取人数太多 =5002,
        /// <summary>
        /// 领取人数太少
        /// </summary>
        领取人数太少 =5003,
        /// <summary>
        /// 不是汉字
        /// </summary>
        不是汉字 =5004,
        /// <summary>
        /// 呼叫者手机号为空
        /// </summary>
        呼叫者手机号为空 = 3003,
        /// <summary>
        /// 被呼叫者手机号为空
        /// </summary>
        被呼叫者手机号为空 = 3004
    }
  
}
