using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DTOGeneralResponse
{

    public string Message { get; set; }
    public uint Status { get; set; }
    public string ErrorType { get; set; }

    public DTOGeneralResponse(string Message, uint Status, string ErrorType)
    {

        this.Message = Message;
        this.Status = Status;
        this.ErrorType = ErrorType;
    }
}
namespace ConnectionLayer
{
  
}
