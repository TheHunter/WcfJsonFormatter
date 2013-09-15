using System.Collections.Generic;
using System.ServiceModel;
using NHibernate;
using NHibernate.Criterion;
using System.Linq;
using PersistentLayer.Domain;
using PersistentLayer.NHibernate;
using PersistentLayer.NHibernate.WCF;

namespace WcfJsonService.Example
{
    [NhServiceBehavior("DefaultSessionFactory", typeof(WcfServiceHolder))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class SalesService
        : ISalesService
    {

        private readonly INhPagedDAO customPagedDAO;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customPagedDAO"></param>
        public SalesService(INhPagedDAO customPagedDAO)
        {
            this.customPagedDAO = customPagedDAO;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Agency GetAgency(long id)
        {
            Agency ag = customPagedDAO.FindBy<Agency, long>(1, LockMode.Read);
            return ag;
        }

        public Agency GetFirstAgency(int pageIndex, int pageSize)
        {
            Agency ag = new Agency();
            ag.IDManager = 100;
            ag.Name = "ceriotti";
            ag.ID = 15;
            return ag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Salesman GetSalesman(long id)
        {
            Salesman sal = customPagedDAO.FindBy<Salesman, long>(id, LockMode.Read );
            return sal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Salesman GetFirstSalesman(int pageIndex, int pageSize)
        {
            Salesman sal = customPagedDAO.FindBy<Salesman, long>(1, LockMode.Read);
            return sal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Salesman> GetPagedSalesman(int pageIndex, int pageSize)
        {
            var result = this.customPagedDAO
                             .GetIndexPagedResult<Salesman>(pageIndex, pageSize, DetachedCriteria.For<Salesman>());

            return result.GetResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<TradeContract> GetPagedContract(int pageIndex, int pageSize)
        {
            var result = this.customPagedDAO
                             .GetIndexPagedResult<TradeContract>(pageIndex, pageSize,
                                                                 DetachedCriteria.For<TradeContract>());

            return result.GetResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public Salesman UpdateCode(Salesman instance, int code)
        {
            if (instance == null)
                return null;

            instance.IdentityCode = code;
            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contracts"></param>
        /// <returns></returns>
        public bool VerifyContracts(IEnumerable<TradeContract> contracts)
        {
            return (contracts != null && contracts.Any());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="number"></param>
        public void SaveCode(TradeContract contract, long number)
        {
            if (contract != null)
                contract.Number = number;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TradeContract GetContract(int id)
        {
            return customPagedDAO.FindBy<TradeContract, long?>(id, LockMode.Read);
        }
    }
}
