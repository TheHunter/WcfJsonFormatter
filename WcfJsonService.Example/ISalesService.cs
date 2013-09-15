using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using PersistentLayer.Domain;

namespace WcfJsonService.Example
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(WcfServiceHolder))]
    public interface ISalesService
    {
        [OperationContract]
        Agency GetAgency(long id);

        [OperationContract]
        Agency GetFirstAgency(int pageIndex, int pageSize);

        //[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest), OperationContract]
        [OperationContract]
        Salesman GetSalesman(long id);

        //[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest), OperationContract]
        [OperationContract]
        Salesman GetFirstSalesman(int pageIndex, int pageSize);

        //[WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest), OperationContract]
        [OperationContract]
        IEnumerable<Salesman> GetPagedSalesman(int pageIndex, int pageSize);

        [OperationContract]
        IEnumerable<TradeContract> GetPagedContract(int pageIndex, int pageSize);

        [OperationContract]
        Salesman UpdateCode(Salesman instance, int code);

        [OperationContract]
        bool VerifyContracts(IEnumerable<TradeContract> contracts);

        [OperationContract]
        void SaveCode(TradeContract contract, long number);

        [OperationContract]
        TradeContract GetContract(int id);
    }

}
