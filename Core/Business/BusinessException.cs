using System;
namespace Core.Business
{
	public class BusinessException:Exception
	{
        public BusinessException(string message) : base(message)
        {
        }
    }
}

