using System;
using Scaglia.Entity.Model;
using Scaglia.Entity.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scaglia.Entity.Infrastructure;

namespace Scaglia.Entity.Controller
{
    public class EntityController : IEntityController
    {
        public IEntityRepository EntityRepository { get; set; }

        public EntityController(IEntityRepository entityRepository)
        {
            this.EntityRepository = entityRepository;
        }

        public async Task<IEntity> GetById(int idSystem, int idOrganization, int id)
        {
            return await Task.Factory.StartNew(() =>
            {
                IEntity result = null;

                this.EntityRepository.GetById(idSystem, idOrganization, id, new GetByIdCallback()
                {
                    Found = (idSystem_, idOrganization_, id_, entity) =>
                    {
                        result = entity;
                    },
                    NotFound = (idSystem_, idOrganization_, id_) =>
                    {
                        this.AddLog(idSystem_, idOrganization_,
                            string.Format(Constants.EntityNotFoundMessage1,
                            id_.ToString()));
                    },
                    Exception = (idSystem_, idOrganization_, entity_, exception_) =>
                    {
                        this.AddLog(idSystem_, idOrganization_,
                            string.Format(Constants.GetByIdException1,
                            new object[] { Environment.NewLine, id.ToString(), exception_.Message, exception_.StackTrace }));
                    }
                });

                return result;

            }, TaskCreationOptions.None).ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    return task.Result;
                }
                return default(IEntity);
            });
        }

        public async Task<IEntity> GetByCode(int idSystem, int idOrganization, string code)
        {
            return await Task.Factory.StartNew(() =>
            {
                IEntity result = null;

                this.EntityRepository.GetByCode(idSystem, idOrganization, code, new GetByCodeCallback()
                {
                    Found = (idSystem_, idOrganization_, id_, entity) =>
                    {
                        result = entity;
                    },
                    NotFound = (idSystem_, idOrganization_, id_) =>
                    {
                        this.AddLog(idSystem_, idOrganization_,
                            string.Format(Constants.EntityNotFoundMessage2,
                            id_.ToString()));
                    },
                    Exception = (idSystem_, idOrganization_, entity_, exception_) =>
                    {
                        this.AddLog(idSystem_, idOrganization_,
                            string.Format(Constants.GetByIdException2,
                            new object[] { Environment.NewLine, code, exception_.Message, exception_.StackTrace }));
                    }
                });

                return result;

            }, TaskCreationOptions.None).ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    return task.Result;
                }
                return default(IEntity);
            });
        }

        public async Task<IList<IEntity>> GetByAttributes(int idSystem, int idOrganization, IList<IAttribute> attributes)
        {
            return await Task.Factory.StartNew(() =>
            {
                IList<IEntity> result = null;

                this.EntityRepository.GetByAttributes(idSystem, idOrganization, attributes, new GetByAttributesCallback()
                {
                    Found = (idSystem_, idOrganization_, attributes_, entities_) =>
                    {
                        result = entities_;
                    },
                    NotFound = (idSystem_, idOrganization_, attributes_) =>
                    {
                        this.AddLog(idSystem_, idOrganization_, Constants.EntitiesNotFoundMessage1);
                    },
                    Exception = (idSystem_, idOrganization_, attributes_, exception_) =>
                    {
                        this.AddLog(idSystem_, idOrganization_,
                       string.Format(Constants.EntitiesExceptionMessage1,
                       new object[] { Environment.NewLine, exception_.Message, exception_.StackTrace }));
                    }
                });

                return result;

            }, TaskCreationOptions.None).ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    return task.Result;
                }
                return default(IList<IEntity>);
            });
        }

        public async Task<IEntity> Create(int idSystem, int idOrganization, IEntity entity)
        {
            return await Task.Factory.StartNew(() =>
            {
                IEntity result = null;

                this.EntityRepository.Create(idSystem, idOrganization, entity, new CreateCallback()
                {
                    Created = (idSystem_, idOrganization_, entity_) =>
                    {
                        result = entity_;
                    },
                    NotCreated = (idSystem_, idOrganization_, entity_) =>
                    {
                        this.AddLog(idSystem_, idOrganization_, Constants.EntityNotCreatedMessage1);
                    },
                    Exception = (idSystem_, idOrganization_, entity_, exception_) =>
                    {
                        this.AddLog(idSystem_, idOrganization_,
                          string.Format(Constants.CreateException1,
                          new object[] { Environment.NewLine, exception_.Message, exception_.StackTrace }));
                    }
                });

                return result;

            }, TaskCreationOptions.None).ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    return task.Result;
                }
                return default(IEntity);
            });
        }

        public async Task<IEntity> Update(int idSystem, int idOrganization, IEntity entity)
        {
            return await Task.Factory.StartNew(() =>
            {
                IEntity result = null;

                this.EntityRepository.Update(idSystem, idOrganization, entity, new UpdateCallback()
                {
                    Updated = (idSystem_, idOrganization_, entity_) =>
                    {
                        result = entity_;
                    },
                    NotUpdated = (idSystem_, idOrganization_, entity_) =>
                    {
                        this.AddLog(idSystem_, idOrganization_, Constants.EntityNotUpdatedMessage1);
                    },
                    Exception = (idSystem_, idOrganization_, entity_, exception_) =>
                    {
                        this.AddLog(idSystem_, idOrganization_,
                          string.Format(Constants.UpdateException1,
                          new object[] { Environment.NewLine, exception_.Message, exception_.StackTrace }));
                    }
                });

                return result;

            }, TaskCreationOptions.None).ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    return task.Result;
                }
                return default(IEntity);
            });
        }

        public async Task<Boolean> Delete(int idSystem, int idOrganization, IEntity entity)
        {
            return await Task.Factory.StartNew(() =>
            {
                bool result = false;

                this.EntityRepository.Delete(idSystem, idOrganization, entity, new DeleteCallback()
                {
                    Deleted = (idSystem_, idOrganization_, entity_) =>
                    {
                        result = true;
                    },
                    NotDeleted = (idSystem_, idOrganization_, entity_) =>
                    {
                        this.AddLog(idSystem_, idOrganization_, Constants.EntityNotDeletedMessage1);
                    },
                    Exception = (idSystem_, idOrganization_, entity_, exception_) =>
                    {
                        this.AddLog(idSystem_, idOrganization_,
                          string.Format(Constants.DeleteException1,
                          new object[] { Environment.NewLine, exception_.Message, exception_.StackTrace }));
                    }
                });

                return result;

            }, TaskCreationOptions.None).ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    return task.Result;
                }
                return false;
            });
        }

        private void AddLog(int idSystem, int idOrganization, String message)
        {
            String formattedMessage = string.Format(Constants.LogMessage1,
                new object[] { Environment.NewLine, DateTime.Now, idSystem, idOrganization, message });

            // Criar uma nova entidade de log
            IEntity newEntity = new Model.Entity()
            {
                IdSystem = idSystem,
                IdOrganization = idOrganization,
                Name = Constants.LOG,
                Code = RNGKey.New(),
                EntityAttributes = new List<IEntityAttribute>()
            };
            newEntity.EntityAttributes.Add(new EntityAttribute()
            {
                Name = Constants.MESSAGE,
                Type = EntityAttributeType.TinyString,
                EntityAttributeValue = new EntityAttributeValue()
                {
                    Value = formattedMessage
                }
            });

            this.EntityRepository.Create(idSystem, idOrganization, newEntity,
            new CreateCallback()
            {
                Created = (idSystem_, idOrganization_, entity_) => { },
                NotCreated = (idSystem_, idOrganization_, entity_) => { },
                Exception = (idSystem_, idOrganization_, entity_, exception_) => { }
            });
        }
    }
}
