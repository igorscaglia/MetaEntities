using System;
using Scaglia.Entity.DAL;
using Scaglia.Entity.Model;
using System.Collections.Generic;
using System.Linq;
using Scaglia.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Reflection;

namespace Scaglia.Entity.Repository.MySql
{
    public sealed class MySqlEntityRepository : IEntityRepository
    {
        public MySqlEntityRepository()
        {
            this.CreateFilterEntityAttributeValueDictionary();
            this.CreateGetEntityAttributeValueDictionary();
            this.CreateNewEntityAttributeValue();
        }

        #region "Dicionários para lidar com os tipos"

        private Dictionary<EntityAttributeType, Func<Int32, IEntityAttributeValue>>
            GetEntityAttributeValueDictionary
        { get; set; }

        private Dictionary<EntityAttributeType, Func<MySqlEntityDatabaseContext, EntityAttributeTable, IAttribute, Boolean>>
            FilterEntityAttributeValueDictionary
        { get; set; }

        private Dictionary<EntityAttributeType, Action<EntityAttributeTable, IEntityAttributeValue>>
            NewEntityAttributeValueDictionary
        { get; set; }

        private void CreateFilterEntityAttributeValueDictionary()
        {
            this.FilterEntityAttributeValueDictionary = new Dictionary<EntityAttributeType, Func<MySqlEntityDatabaseContext, EntityAttributeTable, IAttribute, Boolean>>();

            this.FilterEntityAttributeValueDictionary.Add(EntityAttributeType.Blob, (db, ea, a) =>
            {
                // Se não passou operador então é Equals
                if (a.Operator == null)
                {
                    a.Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals);
                }

                // parametro do atributo da entidade
                var entityAttributeParam = Expression.Parameter(typeof(BlobEntityAttributeTable), Constants.ENTITY_ATTRIBUTE);

                // where do id do valor do atributo
                var idEntityAttributeProperty = Expression.Property(entityAttributeParam, typeof(BlobEntityAttributeTable).GetProperty(Constants.ID_ENTITY_ATTRIBUTE));
                var idEntityAttributeConstant = Expression.Constant(ea.Id, typeof(Int32?));
                Expression idEntityAttributeEqualExpression = Expression.Equal(idEntityAttributeProperty, idEntityAttributeConstant);

                // Create an expression tree that represents the expression entityAttribute1.Name == 'xxx'
                var valueProperty = Expression.Property(entityAttributeParam, typeof(BlobEntityAttributeTable).GetProperty(Constants.VALUE));
                ConstantExpression value1Constant = null;

                // where do valor
                Expression valueExpression = null;

                // Operador que foi passado pelo parâmetro IAttribute
                AttributeOperator attributeOperator = default(AttributeOperator);

                if (Enum.TryParse<AttributeOperator>(a.Operator, out attributeOperator))
                {
                    switch (attributeOperator)
                    {
                        // Blob só usa Equals
                        case AttributeOperator.GraterThanOrEqual:
                        case AttributeOperator.LessThanOrEqual:
                        case AttributeOperator.Between:
                        case AttributeOperator.GraterThan:
                        case AttributeOperator.LessThan:
                        case AttributeOperator.StartsWith:
                        case AttributeOperator.Contains:
                        case AttributeOperator.Equals:
                            value1Constant = System.Linq.Expressions.Expression.Constant(a.Value1, typeof(byte[]));
                            valueExpression = Expression.Equal(valueProperty, value1Constant);
                            break;
                        default:
                            break;
                    }
                }

                Expression finalWhereExpression = Expression.AndAlso(idEntityAttributeEqualExpression, valueExpression);
                IQueryable<BlobEntityAttributeTable> entityAttributeQueryable = db.BlobEntityAttributeTable.AsQueryable();
                MethodCallExpression whereCallExpression = Expression.Call(typeof(Queryable),
                    Constants.WHERE,
                    new Type[] { entityAttributeQueryable.ElementType },
                    entityAttributeQueryable.Expression,
                    Expression.Lambda<Func<BlobEntityAttributeTable, bool>>(
                    finalWhereExpression,
                    new ParameterExpression[] { entityAttributeParam })
                );

                var entityAttributesQuery = entityAttributeQueryable.Provider.CreateQuery<BlobEntityAttributeTable>(whereCallExpression);

                // Retorna se o valor foi comparado
                return entityAttributesQuery.Count() > 0;
            });

            this.FilterEntityAttributeValueDictionary.Add(EntityAttributeType.Boolean, (db, ea, a) =>
            {
                // Se não passou operador então é Equals
                if (a.Operator == null)
                {
                    a.Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals);
                }

                // parametro do atributo da entidade
                var entityAttributeParam = Expression.Parameter(typeof(BooleanEntityAttributeTable), Constants.ENTITY_ATTRIBUTE);

                // where do id do valor do atributo
                var idEntityAttributeProperty = Expression.Property(entityAttributeParam, typeof(BooleanEntityAttributeTable).GetProperty(Constants.ID_ENTITY_ATTRIBUTE));
                var idEntityAttributeConstant = Expression.Constant(ea.Id, typeof(Int32?));
                Expression idEntityAttributeEqualExpression = Expression.Equal(idEntityAttributeProperty, idEntityAttributeConstant);

                // Create an expression tree that represents the expression entityAttribute1.Name == 'xxx'
                var valueProperty = Expression.Property(entityAttributeParam, typeof(BooleanEntityAttributeTable).GetProperty(Constants.VALUE));
                ConstantExpression value1Constant = null;

                // where do valor
                Expression valueExpression = null;

                // Operador que foi passado pelo parâmetro IAttribute
                AttributeOperator attributeOperator = default(AttributeOperator);

                if (Enum.TryParse<AttributeOperator>(a.Operator, out attributeOperator))
                {
                    switch (attributeOperator)
                    {
                        // Boolean só usa Equals
                        case AttributeOperator.GraterThanOrEqual:
                        case AttributeOperator.LessThanOrEqual:
                        case AttributeOperator.Between:
                        case AttributeOperator.GraterThan:
                        case AttributeOperator.LessThan:
                        case AttributeOperator.StartsWith:
                        case AttributeOperator.Contains:
                        case AttributeOperator.Equals:
                            value1Constant = Expression.Constant(Convert.ToBoolean(a.Value1), typeof(Boolean?));
                            valueExpression = Expression.Equal(valueProperty, value1Constant);
                            break;
                        default:
                            break;
                    }
                }

                Expression finalWhereExpression = Expression.AndAlso(idEntityAttributeEqualExpression, valueExpression);
                IQueryable<BooleanEntityAttributeTable> entityAttributeQueryable = db.BooleanEntityAttributeTable.AsQueryable();
                MethodCallExpression whereCallExpression = Expression.Call(typeof(Queryable),
                    Constants.WHERE,
                    new Type[] { entityAttributeQueryable.ElementType },
                    entityAttributeQueryable.Expression,
                    Expression.Lambda<Func<BooleanEntityAttributeTable, bool>>(
                    finalWhereExpression,
                    new ParameterExpression[] { entityAttributeParam })
                );

                var entityAttributesQuery = entityAttributeQueryable.Provider.CreateQuery<BooleanEntityAttributeTable>(whereCallExpression);

                // Retorna se o valor foi comparado
                return entityAttributesQuery.Count() > 0;
            });

            this.FilterEntityAttributeValueDictionary.Add(EntityAttributeType.DateTime, (db, ea, a) =>
            {
                // Se não passou operador então é Equals
                if (a.Operator == null)
                {
                    a.Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals);
                }

                // parametro do atributo da entidade
                var entityAttributeParam = Expression.Parameter(typeof(DateTimeEntityAttributeTable), Constants.ENTITY_ATTRIBUTE);

                // where do id do valor do atributo
                var idEntityAttributeProperty = Expression.Property(entityAttributeParam, typeof(DateTimeEntityAttributeTable).GetProperty(Constants.ID_ENTITY_ATTRIBUTE));
                var idEntityAttributeConstant = Expression.Constant(ea.Id, typeof(Int32?));
                Expression idEntityAttributeEqualExpression = Expression.Equal(idEntityAttributeProperty, idEntityAttributeConstant);

                // Create an expression tree that represents the expression entityAttribute1.Name == 'xxx'
                var valueProperty = Expression.Property(entityAttributeParam, typeof(DateTimeEntityAttributeTable).GetProperty(Constants.VALUE));
                ConstantExpression value1Constant = null;

                // where do valor
                Expression valueExpression = null;

                // Operador que foi passado pelo parâmetro IAttribute
                AttributeOperator attributeOperator = default(AttributeOperator);

                if (Enum.TryParse<AttributeOperator>(a.Operator, out attributeOperator))
                {
                    switch (attributeOperator)
                    {
                        // DateTime não usam StartsWith
                        case AttributeOperator.StartsWith:
                        case AttributeOperator.Equals:
                            value1Constant = Expression.Constant(Convert.ToDateTime(a.Value1), typeof(DateTime?));
                            valueExpression = Expression.Equal(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.LessThan:
                            value1Constant = Expression.Constant(Convert.ToDateTime(a.Value1), typeof(DateTime?));
                            valueExpression = Expression.LessThan(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.GraterThan:
                            value1Constant = Expression.Constant(Convert.ToDateTime(a.Value1), typeof(DateTime?));
                            valueExpression = Expression.GreaterThan(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.Between:
                            // VALUE >= VALUE1 AND VALUE <= VALUE2
                            value1Constant = Expression.Constant(Convert.ToDateTime(a.Value1), typeof(DateTime?));
                            var value2Constant = Expression.Constant(Convert.ToDateTime(a.Value2), typeof(DateTime?));
                            var value1GreaterThan = Expression.GreaterThanOrEqual(valueProperty, value1Constant);
                            var value2LessThan = Expression.LessThanOrEqual(valueProperty, value2Constant);
                            valueExpression = Expression.And(value1GreaterThan, value2LessThan);
                            break;
                        case AttributeOperator.LessThanOrEqual:
                            value1Constant = Expression.Constant(Convert.ToDateTime(a.Value1), typeof(DateTime?));
                            valueExpression = Expression.LessThanOrEqual(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.GraterThanOrEqual:
                            value1Constant = Expression.Constant(Convert.ToDateTime(a.Value1), typeof(DateTime?));
                            valueExpression = Expression.GreaterThanOrEqual(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.Contains:
                            List<DateTime?> values = new List<DateTime?>();

                            // Vamos converter os valores
                            foreach (var value in ((String)a.Value1).Split(';'))
                            {
                                values.Add(Convert.ToDateTime(value));
                            }
                            value1Constant = Expression.Constant(values.ToArray(), typeof(DateTime?[]));
                            var containsMethods = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(aaa => aaa.Name == "Contains");
                            MethodInfo methodInfo = null;
                            foreach (var method in containsMethods)
                            {
                                if (method.GetParameters().Count() == 2)
                                {
                                    methodInfo = method;
                                    break;
                                }
                            }
                            methodInfo = methodInfo.MakeGenericMethod(valueProperty.Type);
                            valueExpression = Expression.Call(methodInfo, new Expression[] { value1Constant, valueProperty });
                            break;
                        default:
                            break;
                    }
                }

                Expression finalWhereExpression = Expression.AndAlso(idEntityAttributeEqualExpression, valueExpression);
                IQueryable<DateTimeEntityAttributeTable> entityAttributeQueryable = db.DateTimeEntityAttributeTable.AsQueryable();
                MethodCallExpression whereCallExpression = Expression.Call(typeof(Queryable),
                    Constants.WHERE,
                    new Type[] { entityAttributeQueryable.ElementType },
                    entityAttributeQueryable.Expression,
                    Expression.Lambda<Func<DateTimeEntityAttributeTable, bool>>(
                    finalWhereExpression,
                    new ParameterExpression[] { entityAttributeParam })
                );

                var entityAttributesQuery = entityAttributeQueryable.Provider.CreateQuery<DateTimeEntityAttributeTable>(whereCallExpression);

                // Retorna se o valor foi comparado
                return entityAttributesQuery.Count() > 0;
            });

            this.FilterEntityAttributeValueDictionary.Add(EntityAttributeType.Double, (db, ea, a) =>
            {
                // Se não passou operador então é Equals
                if (a.Operator == null)
                {
                    a.Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals);
                }

                // parametro do atributo da entidade
                var entityAttributeParam = Expression.Parameter(typeof(DoubleEntityAttributeTable), Constants.ENTITY_ATTRIBUTE);

                // where do id do valor do atributo
                var idEntityAttributeProperty = Expression.Property(entityAttributeParam, typeof(DoubleEntityAttributeTable).GetProperty(Constants.ID_ENTITY_ATTRIBUTE));
                var idEntityAttributeConstant = Expression.Constant(ea.Id, typeof(Int32?));
                Expression idEntityAttributeEqualExpression = Expression.Equal(idEntityAttributeProperty, idEntityAttributeConstant);

                // Create an expression tree that represents the expression entityAttribute1.Name == 'xxx'
                var valueProperty = Expression.Property(entityAttributeParam, typeof(DoubleEntityAttributeTable).GetProperty(Constants.VALUE));
                ConstantExpression value1Constant = null;

                // where do valor
                Expression valueExpression = null;

                // Operador que foi passado pelo parâmetro IAttribute
                AttributeOperator attributeOperator = default(AttributeOperator);

                if (Enum.TryParse<AttributeOperator>(a.Operator, out attributeOperator))
                {
                    switch (attributeOperator)
                    {
                        // DateTime não usam StartsWith
                        case AttributeOperator.StartsWith:
                        case AttributeOperator.Equals:
                            value1Constant = Expression.Constant(Convert.ToDouble(a.Value1), typeof(Double?));
                            valueExpression = Expression.Equal(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.LessThan:
                            value1Constant = Expression.Constant(Convert.ToDouble(a.Value1), typeof(Double?));
                            valueExpression = Expression.LessThan(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.GraterThan:
                            value1Constant = Expression.Constant(Convert.ToDouble(a.Value1), typeof(Double?));
                            valueExpression = Expression.GreaterThan(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.Between:
                            // VALUE >= VALUE1 AND VALUE <= VALUE2
                            value1Constant = Expression.Constant(Convert.ToDouble(a.Value1), typeof(Double?));
                            var value2Constant = Expression.Constant(Convert.ToDouble(a.Value2), typeof(Double?));
                            var value1GreaterThan = Expression.GreaterThanOrEqual(valueProperty, value1Constant);
                            var value2LessThan = Expression.LessThanOrEqual(valueProperty, value2Constant);
                            valueExpression = Expression.And(value1GreaterThan, value2LessThan);
                            break;
                        case AttributeOperator.LessThanOrEqual:
                            value1Constant = Expression.Constant(Convert.ToDouble(a.Value1), typeof(Double?));
                            valueExpression = Expression.LessThanOrEqual(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.GraterThanOrEqual:
                            value1Constant = Expression.Constant(Convert.ToDouble(a.Value1), typeof(Double?));
                            valueExpression = Expression.GreaterThanOrEqual(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.Contains:
                            List<Double?> values = new List<Double?>();

                            // Vamos converter os valores
                            foreach (var value in ((String)a.Value1).Split(';'))
                            {
                                values.Add(Convert.ToDouble(value));
                            }
                            value1Constant = Expression.Constant(values.ToArray(), typeof(Double?[]));
                            var containsMethods = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(aaa => aaa.Name == "Contains");
                            MethodInfo methodInfo = null;
                            foreach (var method in containsMethods)
                            {
                                if (method.GetParameters().Count() == 2)
                                {
                                    methodInfo = method;
                                    break;
                                }
                            }
                            methodInfo = methodInfo.MakeGenericMethod(valueProperty.Type);
                            valueExpression = Expression.Call(methodInfo, new Expression[] { value1Constant, valueProperty });
                            break;
                        default:
                            break;
                    }
                }

                Expression finalWhereExpression = Expression.AndAlso(idEntityAttributeEqualExpression, valueExpression);
                IQueryable<DoubleEntityAttributeTable> entityAttributeQueryable = db.DoubleEntityAttributeTable.AsQueryable();
                MethodCallExpression whereCallExpression = Expression.Call(typeof(Queryable),
                    Constants.WHERE,
                    new Type[] { entityAttributeQueryable.ElementType },
                    entityAttributeQueryable.Expression,
                    Expression.Lambda<Func<DoubleEntityAttributeTable, bool>>(
                    finalWhereExpression,
                    new ParameterExpression[] { entityAttributeParam })
                );

                var entityAttributesQuery = entityAttributeQueryable.Provider.CreateQuery<DoubleEntityAttributeTable>(whereCallExpression);

                // Retorna se o valor foi comparado
                return entityAttributesQuery.Count() > 0;
            });

            this.FilterEntityAttributeValueDictionary.Add(EntityAttributeType.Float, (db, ea, a) =>
            {
                // Se não passou operador então é Equals
                if (a.Operator == null)
                {
                    a.Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals);
                }

                // parametro do atributo da entidade
                var entityAttributeParam = Expression.Parameter(typeof(FloatEntityAttributeTable), Constants.ENTITY_ATTRIBUTE);

                // where do id do valor do atributo
                var idEntityAttributeProperty = Expression.Property(entityAttributeParam, typeof(FloatEntityAttributeTable).GetProperty(Constants.ID_ENTITY_ATTRIBUTE));
                var idEntityAttributeConstant = Expression.Constant(ea.Id, typeof(Int32?));
                Expression idEntityAttributeEqualExpression = Expression.Equal(idEntityAttributeProperty, idEntityAttributeConstant);

                // Create an expression tree that represents the expression entityAttribute1.Name == 'xxx'
                var valueProperty = Expression.Property(entityAttributeParam, typeof(FloatEntityAttributeTable).GetProperty(Constants.VALUE));
                ConstantExpression value1Constant = null;

                // where do valor
                Expression valueExpression = null;

                // Operador que foi passado pelo parâmetro IAttribute
                AttributeOperator attributeOperator = default(AttributeOperator);

                if (Enum.TryParse<AttributeOperator>(a.Operator, out attributeOperator))
                {
                    switch (attributeOperator)
                    {
                        // DateTime não usam StartsWith
                        case AttributeOperator.StartsWith:
                        case AttributeOperator.Equals:
                            value1Constant = Expression.Constant(Convert.ToSingle(a.Value1), typeof(Single?));
                            valueExpression = Expression.Equal(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.LessThan:
                            value1Constant = Expression.Constant(Convert.ToSingle(a.Value1), typeof(Single?));
                            valueExpression = Expression.LessThan(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.GraterThan:
                            value1Constant = Expression.Constant(Convert.ToSingle(a.Value1), typeof(Single?));
                            valueExpression = Expression.GreaterThan(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.Between:
                            // VALUE >= VALUE1 AND VALUE <= VALUE2
                            value1Constant = Expression.Constant(Convert.ToSingle(a.Value1), typeof(Single?));
                            var value2Constant = Expression.Constant(Convert.ToSingle(a.Value2), typeof(Single?));
                            var value1GreaterThan = Expression.GreaterThanOrEqual(valueProperty, value1Constant);
                            var value2LessThan = Expression.LessThanOrEqual(valueProperty, value2Constant);
                            valueExpression = Expression.And(value1GreaterThan, value2LessThan);
                            break;
                        case AttributeOperator.LessThanOrEqual:
                            value1Constant = Expression.Constant(Convert.ToSingle(a.Value1), typeof(Single?));
                            valueExpression = Expression.LessThanOrEqual(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.GraterThanOrEqual:
                            value1Constant = Expression.Constant(Convert.ToSingle(a.Value1), typeof(Single?));
                            valueExpression = Expression.GreaterThanOrEqual(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.Contains:
                            List<Single?> values = new List<Single?>();

                            // Vamos converter os valores
                            foreach (var value in ((String)a.Value1).Split(';'))
                            {
                                values.Add(Convert.ToSingle(value));
                            }
                            value1Constant = Expression.Constant(values.ToArray(), typeof(Single?[]));
                            var containsMethods = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(aaa => aaa.Name == "Contains");
                            MethodInfo methodInfo = null;
                            foreach (var method in containsMethods)
                            {
                                if (method.GetParameters().Count() == 2)
                                {
                                    methodInfo = method;
                                    break;
                                }
                            }
                            methodInfo = methodInfo.MakeGenericMethod(valueProperty.Type);
                            valueExpression = Expression.Call(methodInfo, new Expression[] { value1Constant, valueProperty });
                            break;
                        default:
                            break;
                    }
                }

                Expression finalWhereExpression = Expression.AndAlso(idEntityAttributeEqualExpression, valueExpression);
                IQueryable<FloatEntityAttributeTable> entityAttributeQueryable = db.FloatEntityAttributeTable.AsQueryable();
                MethodCallExpression whereCallExpression = Expression.Call(typeof(Queryable),
                    Constants.WHERE,
                    new Type[] { entityAttributeQueryable.ElementType },
                    entityAttributeQueryable.Expression,
                    Expression.Lambda<Func<FloatEntityAttributeTable, bool>>(
                    finalWhereExpression,
                    new ParameterExpression[] { entityAttributeParam })
                );

                var entityAttributesQuery = entityAttributeQueryable.Provider.CreateQuery<FloatEntityAttributeTable>(whereCallExpression);

                // Retorna se o valor foi comparado
                return entityAttributesQuery.Count() > 0;
            });

            this.FilterEntityAttributeValueDictionary.Add(EntityAttributeType.Integer, (db, ea, a) =>
            {
                // Se não passou operador então é Equals
                if (a.Operator == null)
                {
                    a.Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals);
                }

                // parametro do atributo da entidade
                var entityAttributeParam = Expression.Parameter(typeof(Scaglia.Entity.DAL.IntegerEntityAttributeTable), Constants.ENTITY_ATTRIBUTE);

                // where do id do valor do atributo
                var idEntityAttributeProperty = Expression.Property(entityAttributeParam, typeof(IntegerEntityAttributeTable).GetProperty(Constants.ID_ENTITY_ATTRIBUTE));
                var idEntityAttributeConstant = Expression.Constant(ea.Id, typeof(Int32?));
                Expression idEntityAttributeEqualExpression = Expression.Equal(idEntityAttributeProperty, idEntityAttributeConstant);

                // Create an expression tree that represents the expression entityAttribute1.Name == 'xxx'
                var valueProperty = Expression.Property(entityAttributeParam, typeof(Scaglia.Entity.DAL.IntegerEntityAttributeTable).GetProperty(Constants.VALUE));
                ConstantExpression value1Constant = null;

                // where do valor
                Expression valueExpression = null;

                // Operador que foi passado pelo parâmetro IAttribute
                AttributeOperator attributeOperator = default(AttributeOperator);

                if (Enum.TryParse<AttributeOperator>(a.Operator, out attributeOperator))
                {
                    switch (attributeOperator)
                    {
                        // Números não usam StartsWith
                        case AttributeOperator.StartsWith:
                        case AttributeOperator.Equals:
                            value1Constant = Expression.Constant(Convert.ToInt32(a.Value1), typeof(Int32?));
                            valueExpression = Expression.Equal(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.LessThan:
                            value1Constant = Expression.Constant(Convert.ToInt32(a.Value1), typeof(Int32?));
                            valueExpression = Expression.LessThan(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.GraterThan:
                            value1Constant = Expression.Constant(Convert.ToInt32(a.Value1), typeof(Int32?));
                            valueExpression = Expression.GreaterThan(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.Between:
                            // VALUE >= VALUE1 AND VALUE <= VALUE2
                            value1Constant = Expression.Constant(Convert.ToInt32(a.Value1), typeof(Int32?));
                            var value2Constant = Expression.Constant(Convert.ToInt32(a.Value2), typeof(Int32?));
                            var value1GreaterThan = Expression.GreaterThanOrEqual(valueProperty, value1Constant);
                            var value2LessThan = Expression.LessThanOrEqual(valueProperty, value2Constant);
                            valueExpression = Expression.And(value1GreaterThan, value2LessThan);
                            break;
                        case AttributeOperator.LessThanOrEqual:
                            value1Constant = Expression.Constant(Convert.ToInt32(a.Value1), typeof(Int32?));
                            valueExpression = Expression.LessThanOrEqual(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.GraterThanOrEqual:
                            value1Constant = Expression.Constant(Convert.ToInt32(a.Value1), typeof(Int32?));
                            valueExpression = Expression.GreaterThanOrEqual(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.Contains:
                            List<Int32?> values = new List<int?>();

                            // Vamos converter os valores
                            foreach (var value in ((String)a.Value1).Split(';'))
                            {
                                values.Add(Convert.ToInt32(value));
                            }
                            value1Constant = Expression.Constant(values.ToArray(), typeof(Int32?[]));
                            var containsMethods = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(aaa => aaa.Name == "Contains");
                            MethodInfo methodInfo = null;
                            foreach (var method in containsMethods)
                            {
                                if (method.GetParameters().Count() == 2)
                                {
                                    methodInfo = method;
                                    break;
                                }
                            }
                            methodInfo = methodInfo.MakeGenericMethod(valueProperty.Type);
                            valueExpression = Expression.Call(methodInfo, new Expression[] { value1Constant, valueProperty });
                            break;
                        default:
                            break;
                    }
                }

                Expression finalWhereExpression = Expression.AndAlso(idEntityAttributeEqualExpression, valueExpression);
                IQueryable<IntegerEntityAttributeTable> entityAttributeQueryable = db.IntegerEntityAttributeTable.AsQueryable();
                MethodCallExpression whereCallExpression = Expression.Call(typeof(Queryable),
                    Constants.WHERE,
                    new Type[] { entityAttributeQueryable.ElementType },
                    entityAttributeQueryable.Expression,
                    Expression.Lambda<Func<IntegerEntityAttributeTable, bool>>(
                    finalWhereExpression,
                    new ParameterExpression[] { entityAttributeParam })
                );

                var entityAttributesQuery = entityAttributeQueryable.Provider.CreateQuery<IntegerEntityAttributeTable>(whereCallExpression);

                // Retorna se o valor foi comparado
                return entityAttributesQuery.Count() > 0;
            });

            this.FilterEntityAttributeValueDictionary.Add(EntityAttributeType.MediumString, (db, ea, a) =>
            {
                // Se não passou operador então é Equals
                if (a.Operator == null)
                {
                    a.Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals);
                }

                // parametro do atributo da entidade
                var entityAttributeParam = Expression.Parameter(typeof(MediumStringEntityAttributeTable), Constants.ENTITY_ATTRIBUTE);

                // where do id do atributo
                var idEntityAttributeProperty = Expression.Property(entityAttributeParam, typeof(MediumStringEntityAttributeTable).GetProperty(Constants.ID_ENTITY_ATTRIBUTE));
                var idEntityAttributeConstant = Expression.Constant(ea.Id, typeof(Int32?));
                Expression idEntityAttributeEqualExpression = Expression.Equal(idEntityAttributeProperty, idEntityAttributeConstant);

                // Create an expression tree that represents the expression entityAttribute1.Name == 'xxx'
                var valueProperty = Expression.Property(entityAttributeParam, typeof(MediumStringEntityAttributeTable).GetProperty(Constants.VALUE));

                ConstantExpression value1Constant = null;
                Expression valueExpression = null;
                MethodInfo methodInfo = null;

                // Operador que foi passado pelo parâmetro IAttribute
                AttributeOperator attributeOperator = default(AttributeOperator);

                if (Enum.TryParse<AttributeOperator>(a.Operator, out attributeOperator))
                {
                    switch (attributeOperator)
                    {
                        // Tipos String não tem comparações numéricas, então serão redirecionados para Equals
                        case AttributeOperator.GraterThanOrEqual:
                        case AttributeOperator.LessThanOrEqual:
                        case AttributeOperator.Between:
                        case AttributeOperator.GraterThan:
                        case AttributeOperator.LessThan:
                        case AttributeOperator.Equals:
                            value1Constant = Expression.Constant(a.Value1, typeof(String));
                            valueExpression = Expression.Equal(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.Contains:
                            value1Constant = Expression.Constant(a.Value1, typeof(String));
                            value1Constant = Expression.Constant(((String)a.Value1).Split(';'), typeof(String[]));
                            var containsMethods = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(aaa => aaa.Name == "Contains");
                            foreach (var method in containsMethods)
                            {
                                if (method.GetParameters().Count() == 2)
                                {
                                    methodInfo = method;
                                    break;
                                }
                            }
                            methodInfo = methodInfo.MakeGenericMethod(valueProperty.Type);
                            valueExpression = Expression.Call(methodInfo, new Expression[] { value1Constant, valueProperty });
                            break;
                        case AttributeOperator.StartsWith:
                            value1Constant = Expression.Constant(a.Value1, typeof(String));
                            var propertyInfo = typeof(MediumStringEntityAttributeTable).GetProperty(Constants.VALUE);
                            var MemberAccess = Expression.MakeMemberAccess(entityAttributeParam, propertyInfo);
                            methodInfo = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                            valueExpression = Expression.Call(MemberAccess, methodInfo, value1Constant);
                            break;
                        default:
                            break;
                    }
                }

                Expression finalWhereExpression = Expression.AndAlso(idEntityAttributeEqualExpression, valueExpression);
                IQueryable<MediumStringEntityAttributeTable> entityAttributeQueryable = db.MediumStringEntityAttributeTable.AsQueryable();
                MethodCallExpression whereCallExpression = Expression.Call(typeof(Queryable),
                    Constants.WHERE,
                    new Type[] { entityAttributeQueryable.ElementType },
                    entityAttributeQueryable.Expression,
                    Expression.Lambda<Func<MediumStringEntityAttributeTable, bool>>(
                    finalWhereExpression,
                    new ParameterExpression[] { entityAttributeParam })
                );

                var entityAttributesQuery = entityAttributeQueryable.Provider.CreateQuery<MediumStringEntityAttributeTable>(whereCallExpression);

                // Retorna se o valor foi comparado 
                return entityAttributesQuery.Count() > 0;
            });

            this.FilterEntityAttributeValueDictionary.Add(EntityAttributeType.String, (db, ea, a) =>
            {
                // Se não passou operador então é Equals
                if (a.Operator == null)
                {
                    a.Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals);
                }

                // parametro do atributo da entidade
                var entityAttributeParam = Expression.Parameter(typeof(StringEntityAttributeTable), Constants.ENTITY_ATTRIBUTE);

                // where do id do atributo
                var idEntityAttributeProperty = Expression.Property(entityAttributeParam, typeof(StringEntityAttributeTable).GetProperty(Constants.ID_ENTITY_ATTRIBUTE));
                var idEntityAttributeConstant = Expression.Constant(ea.Id, typeof(Int32?));
                Expression idEntityAttributeEqualExpression = Expression.Equal(idEntityAttributeProperty, idEntityAttributeConstant);

                // Create an expression tree that represents the expression entityAttribute1.Name == 'xxx'
                var valueProperty = Expression.Property(entityAttributeParam, typeof(StringEntityAttributeTable).GetProperty(Constants.VALUE));

                ConstantExpression value1Constant = null;
                Expression valueExpression = null;
                MethodInfo methodInfo = null;

                // Operador que foi passado pelo parâmetro IAttribute
                AttributeOperator attributeOperator = default(AttributeOperator);

                if (Enum.TryParse<AttributeOperator>(a.Operator, out attributeOperator))
                {
                    switch (attributeOperator)
                    {
                        // Tipos String não tem comparações numéricas, então serão redirecionados para Equals
                        case AttributeOperator.GraterThanOrEqual:
                        case AttributeOperator.LessThanOrEqual:
                        case AttributeOperator.Between:
                        case AttributeOperator.GraterThan:
                        case AttributeOperator.LessThan:
                        case AttributeOperator.Equals:
                            value1Constant = Expression.Constant(a.Value1, typeof(String));
                            valueExpression = Expression.Equal(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.Contains:
                            value1Constant = Expression.Constant(a.Value1, typeof(String));
                            value1Constant = Expression.Constant(((String)a.Value1).Split(';'), typeof(String[]));
                            var containsMethods = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(aaa => aaa.Name == "Contains");
                            foreach (var method in containsMethods)
                            {
                                if (method.GetParameters().Count() == 2)
                                {
                                    methodInfo = method;
                                    break;
                                }
                            }
                            methodInfo = methodInfo.MakeGenericMethod(valueProperty.Type);
                            valueExpression = Expression.Call(methodInfo, new Expression[] { value1Constant, valueProperty });
                            break;
                        case AttributeOperator.StartsWith:
                            value1Constant = Expression.Constant(a.Value1, typeof(String));
                            var propertyInfo = typeof(StringEntityAttributeTable).GetProperty(Constants.VALUE);
                            var MemberAccess = Expression.MakeMemberAccess(entityAttributeParam, propertyInfo);
                            methodInfo = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                            valueExpression = Expression.Call(MemberAccess, methodInfo, value1Constant);
                            break;
                        default:
                            break;
                    }
                }

                Expression finalWhereExpression = Expression.AndAlso(idEntityAttributeEqualExpression, valueExpression);
                IQueryable<StringEntityAttributeTable> entityAttributeQueryable = db.StringEntityAttributeTable.AsQueryable();
                MethodCallExpression whereCallExpression = Expression.Call(typeof(Queryable),
                    Constants.WHERE,
                    new Type[] { entityAttributeQueryable.ElementType },
                    entityAttributeQueryable.Expression,
                    Expression.Lambda<Func<StringEntityAttributeTable, bool>>(
                    finalWhereExpression,
                    new ParameterExpression[] { entityAttributeParam })
                );

                var entityAttributesQuery = entityAttributeQueryable.Provider.CreateQuery<StringEntityAttributeTable>(whereCallExpression);

                // Retorna se o valor foi comparado 
                return entityAttributesQuery.Count() > 0;
            });

            this.FilterEntityAttributeValueDictionary.Add(EntityAttributeType.TinyString, (db, ea, a) =>
            {
                // Se não passou operador então é Equals
                if (a.Operator == null)
                {
                    a.Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals);
                }

                // parametro do atributo da entidade
                var entityAttributeParam = Expression.Parameter(typeof(TinyStringEntityAttributeTable), Constants.ENTITY_ATTRIBUTE);

                // where do id do atributo
                var idEntityAttributeProperty = Expression.Property(entityAttributeParam, typeof(TinyStringEntityAttributeTable).GetProperty(Constants.ID_ENTITY_ATTRIBUTE));
                var idEntityAttributeConstant = Expression.Constant(ea.Id, typeof(Int32?));
                Expression idEntityAttributeEqualExpression = Expression.Equal(idEntityAttributeProperty, idEntityAttributeConstant);

                // Create an expression tree that represents the expression entityAttribute1.Name == 'xxx'
                var valueProperty = Expression.Property(entityAttributeParam, typeof(TinyStringEntityAttributeTable).GetProperty(Constants.VALUE));
                var value1Constant = Expression.Constant(a.Value1, typeof(String));

                Expression valueExpression = null;
                MethodInfo methodInfo = null;

                // Operador que foi passado pelo parâmetro IAttribute
                AttributeOperator attributeOperator = default(AttributeOperator);

                if (Enum.TryParse<AttributeOperator>(a.Operator, out attributeOperator))
                {
                    switch (attributeOperator)
                    {
                        // Tipos String não tem comparações numéricas, então serão redirecionados para Equals
                        case AttributeOperator.GraterThanOrEqual:
                        case AttributeOperator.LessThanOrEqual:
                        case AttributeOperator.Between:
                        case AttributeOperator.GraterThan:
                        case AttributeOperator.LessThan:
                        case AttributeOperator.Equals:
                            valueExpression = Expression.Equal(valueProperty, value1Constant);
                            break;
                        case AttributeOperator.Contains:
                            value1Constant = Expression.Constant(((String)a.Value1).Split(';'), typeof(String[]));
                            var containsMethods = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(aaa => aaa.Name == "Contains");
                            foreach (var method in containsMethods)
                            {
                                if (method.GetParameters().Count() == 2)
                                {
                                    methodInfo = method;
                                    break;
                                }
                            }
                            methodInfo = methodInfo.MakeGenericMethod(valueProperty.Type);
                            valueExpression = Expression.Call(methodInfo, new Expression[] { value1Constant, valueProperty });
                            break;
                        case AttributeOperator.StartsWith:
                            var propertyInfo = typeof(TinyStringEntityAttributeTable).GetProperty(Constants.VALUE);
                            var MemberAccess = Expression.MakeMemberAccess(entityAttributeParam, propertyInfo);
                            methodInfo = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                            valueExpression = Expression.Call(MemberAccess, methodInfo, value1Constant);
                            break;
                        default:
                            break;
                    }
                }

                Expression finalWhereExpression = Expression.AndAlso(idEntityAttributeEqualExpression, valueExpression);
                IQueryable<TinyStringEntityAttributeTable> entityAttributeQueryable = db.TinyStringEntityAttributeTable.AsQueryable();
                MethodCallExpression whereCallExpression = Expression.Call(typeof(Queryable),
                    Constants.WHERE,
                    new Type[] { entityAttributeQueryable.ElementType },
                    entityAttributeQueryable.Expression,
                    Expression.Lambda<Func<TinyStringEntityAttributeTable, bool>>(
                    finalWhereExpression,
                    new ParameterExpression[] { entityAttributeParam })
                );

                var entityAttributesQuery = entityAttributeQueryable.Provider.CreateQuery<TinyStringEntityAttributeTable>(whereCallExpression);

                // Retorna se o valor foi comparado 
                return entityAttributesQuery.Count() > 0;
            });
        }

        private void CreateGetEntityAttributeValueDictionary()
        {
            // Através do tipo desse atributo recuperamos o valor na tabela certa
            this.GetEntityAttributeValueDictionary =
                new Dictionary<EntityAttributeType, Func<Int32, IEntityAttributeValue>>();

            // Nós mudamos aqui e não passa mais o contexto do bd... ver se nos crud vai funcionar

            this.GetEntityAttributeValueDictionary.Add(EntityAttributeType.Blob, (idEntityAttribute) =>
            {
                using (MySqlEntityDatabaseContext context = new MySqlEntityDatabaseContext())
                {

                    var entityAttributeValue = (from a in context.BlobEntityAttributeTable
                                                where
                                                  a.IdEntityAttribute == idEntityAttribute
                                                select a).FirstOrDefault();
                    return new EntityAttributeValue()
                    {
                        Id = entityAttributeValue.Id,
                        IdEntityAttribute = idEntityAttribute,
                        Value = entityAttributeValue.Value
                    };
                }
            });
            this.GetEntityAttributeValueDictionary.Add(EntityAttributeType.Boolean, (idEntityAttribute) =>
            {
                using (MySqlEntityDatabaseContext context = new MySqlEntityDatabaseContext())
                {
                    var entityAttributeValue = (from a in context.BooleanEntityAttributeTable
                                                where
                                                  a.IdEntityAttribute == idEntityAttribute
                                                select a).FirstOrDefault();
                    return new EntityAttributeValue()
                    {
                        Id = entityAttributeValue.Id,
                        IdEntityAttribute = idEntityAttribute,
                        Value = entityAttributeValue.Value
                    };
                }
            });
            this.GetEntityAttributeValueDictionary.Add(EntityAttributeType.DateTime, (idEntityAttribute) =>
            {
                using (MySqlEntityDatabaseContext context = new MySqlEntityDatabaseContext())
                {
                    var entityAttributeValue = (from a in context.DateTimeEntityAttributeTable
                                                where
                                                  a.IdEntityAttribute == idEntityAttribute
                                                select a).FirstOrDefault();
                    return new EntityAttributeValue()
                    {
                        Id = entityAttributeValue.Id,
                        IdEntityAttribute = idEntityAttribute,
                        Value = entityAttributeValue.Value
                    };
                }
            });
            this.GetEntityAttributeValueDictionary.Add(EntityAttributeType.Double, (idEntityAttribute) =>
            {
                using (MySqlEntityDatabaseContext context = new MySqlEntityDatabaseContext())
                {
                    var entityAttributeValue = (from a in context.DoubleEntityAttributeTable
                                                where
                                                  a.IdEntityAttribute == idEntityAttribute
                                                select a).FirstOrDefault();
                    return new EntityAttributeValue()
                    {
                        Id = entityAttributeValue.Id,
                        IdEntityAttribute = idEntityAttribute,
                        Value = entityAttributeValue.Value
                    };
                }
            });
            this.GetEntityAttributeValueDictionary.Add(EntityAttributeType.Float, (idEntityAttribute) =>
            {
                using (MySqlEntityDatabaseContext context = new MySqlEntityDatabaseContext())
                {
                    var entityAttributeValue = (from a in context.FloatEntityAttributeTable
                                                where
                                                  a.IdEntityAttribute == idEntityAttribute
                                                select a).FirstOrDefault();
                    return new EntityAttributeValue()
                    {
                        Id = entityAttributeValue.Id,
                        IdEntityAttribute = idEntityAttribute,
                        Value = entityAttributeValue.Value
                    };
                }
            });
            this.GetEntityAttributeValueDictionary.Add(EntityAttributeType.Integer, (idEntityAttribute) =>
            {
                using (MySqlEntityDatabaseContext context = new MySqlEntityDatabaseContext())
                {
                    var entityAttributeValue = (from a in context.IntegerEntityAttributeTable
                                                where
                                                  a.IdEntityAttribute == idEntityAttribute
                                                select a).FirstOrDefault();

                    return new EntityAttributeValue()
                    {
                        Id = entityAttributeValue.Id,
                        IdEntityAttribute = idEntityAttribute,
                        Value = entityAttributeValue.Value
                    };
                }
            });
            this.GetEntityAttributeValueDictionary.Add(EntityAttributeType.MediumString, (idEntityAttribute) =>
            {
                using (MySqlEntityDatabaseContext context = new MySqlEntityDatabaseContext())
                {
                    var entityAttributeValue = (from a in context.MediumStringEntityAttributeTable
                                                where
                                                  a.IdEntityAttribute == idEntityAttribute
                                                select a).FirstOrDefault();
                    return new EntityAttributeValue()
                    {
                        Id = entityAttributeValue.Id,
                        IdEntityAttribute = idEntityAttribute,
                        Value = entityAttributeValue.Value
                    };
                }
            });
            this.GetEntityAttributeValueDictionary.Add(EntityAttributeType.String, (idEntityAttribute) =>
            {
                using (MySqlEntityDatabaseContext context = new MySqlEntityDatabaseContext())
                {
                    var entityAttributeValue = (from a in context.StringEntityAttributeTable
                                                where
                                                  a.IdEntityAttribute == idEntityAttribute
                                                select a).FirstOrDefault();
                    return new EntityAttributeValue()
                    {
                        Id = entityAttributeValue.Id,
                        IdEntityAttribute = idEntityAttribute,
                        Value = entityAttributeValue.Value
                    };
                }
            });
            this.GetEntityAttributeValueDictionary.Add(EntityAttributeType.TinyString, (idEntityAttribute) =>
            {
                using (MySqlEntityDatabaseContext context = new MySqlEntityDatabaseContext())
                {
                    var entityAttributeValue = (from a in context.TinyStringEntityAttributeTable
                                                where
                                                  a.IdEntityAttribute == idEntityAttribute
                                                select a).FirstOrDefault();
                    return new EntityAttributeValue()
                    {
                        Id = entityAttributeValue.Id,
                        IdEntityAttribute = idEntityAttribute,
                        Value = entityAttributeValue.Value
                    };
                }
            });
        }

        private void CreateNewEntityAttributeValue()
        {
            this.NewEntityAttributeValueDictionary = new Dictionary<EntityAttributeType, Action<EntityAttributeTable, IEntityAttributeValue>>();

            this.NewEntityAttributeValueDictionary.Add(EntityAttributeType.Blob, (entityAttribute, entityAttributeValueToAdd) =>
            {
                IEntityAttributeValue newEntityAttributeValue = new BlobEntityAttributeTable()
                {
                    EntityAttributeTable = entityAttribute
                };
                newEntityAttributeValue.Value = entityAttributeValueToAdd.Value;
            });
            this.NewEntityAttributeValueDictionary.Add(EntityAttributeType.Boolean, (entityAttribute, entityAttributeValueToAdd) =>
            {
                IEntityAttributeValue newEntityAttributeValue = new BooleanEntityAttributeTable()
                {
                    EntityAttributeTable = entityAttribute
                };
                newEntityAttributeValue.Value = Convert.ToBoolean(entityAttributeValueToAdd.Value);
            });
            this.NewEntityAttributeValueDictionary.Add(EntityAttributeType.DateTime, (entityAttribute, entityAttributeValueToAdd) =>
            {
                IEntityAttributeValue newEntityAttributeValue = new DateTimeEntityAttributeTable()
                {
                    EntityAttributeTable = entityAttribute
                };
                newEntityAttributeValue.Value = entityAttributeValueToAdd.Value;
            });
            this.NewEntityAttributeValueDictionary.Add(EntityAttributeType.Double, (entityAttribute, entityAttributeValueToAdd) =>
            {
                IEntityAttributeValue newEntityAttributeValue = new DoubleEntityAttributeTable()
                {
                    EntityAttributeTable = entityAttribute
                };
                newEntityAttributeValue.Value = Convert.ToDouble(entityAttributeValueToAdd.Value);
            });
            this.NewEntityAttributeValueDictionary.Add(EntityAttributeType.Float, (entityAttribute, entityAttributeValueToAdd) =>
            {
                IEntityAttributeValue newEntityAttributeValue = new FloatEntityAttributeTable()
                {
                    EntityAttributeTable = entityAttribute
                };
                newEntityAttributeValue.Value = Convert.ToSingle(entityAttributeValueToAdd.Value);
            });
            this.NewEntityAttributeValueDictionary.Add(EntityAttributeType.Integer, (entityAttribute, entityAttributeValueToAdd) =>
            {
                IEntityAttributeValue newEntityAttributeValue = new IntegerEntityAttributeTable()
                {
                    EntityAttributeTable = entityAttribute
                };
                newEntityAttributeValue.Value = Convert.ToInt32(entityAttributeValueToAdd.Value);
            });
            this.NewEntityAttributeValueDictionary.Add(EntityAttributeType.MediumString, (entityAttribute, entityAttributeValueToAdd) =>
            {
                IEntityAttributeValue newEntityAttributeValue = new MediumStringEntityAttributeTable()
                {
                    EntityAttributeTable = entityAttribute
                };
                newEntityAttributeValue.Value = entityAttributeValueToAdd.Value;
            });
            this.NewEntityAttributeValueDictionary.Add(EntityAttributeType.String, (entityAttribute, entityAttributeValueToAdd) =>
            {
                IEntityAttributeValue newEntityAttributeValue = new StringEntityAttributeTable()
                {
                    EntityAttributeTable = entityAttribute
                };
                newEntityAttributeValue.Value = entityAttributeValueToAdd.Value;
            });
            this.NewEntityAttributeValueDictionary.Add(EntityAttributeType.TinyString, (entityAttribute, entityAttributeValueToAdd) =>
            {
                IEntityAttributeValue newEntityAttributeValue = new TinyStringEntityAttributeTable()
                {
                    EntityAttributeTable = entityAttribute
                };
                newEntityAttributeValue.Value = entityAttributeValueToAdd.Value;
            });
        }

        private IQueryable<EntityTable> GetFilteredEntitiesByIdQueryable(MySqlEntityDatabaseContext context, IAttribute attributeFilter)
        {
            // parametro do atributo da entidade
            var entityTableExpressionParameter = Expression.Parameter(typeof(EntityTable), Constants.ENTITY);
            var idExpressionProperty = Expression.Property(entityTableExpressionParameter, typeof(EntityTable).GetProperty(Constants.ID));

            ConstantExpression value1Constant = null;
            Expression valueExpression = null;

            // Se não passou operador então é Equals
            if (attributeFilter.Operator == null)
            {
                attributeFilter.Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals);
            }

            // Operador que foi passado pelo parâmetro IAttribute
            AttributeOperator attributeOperator = default(AttributeOperator);

            if (Enum.TryParse<AttributeOperator>(attributeFilter.Operator, out attributeOperator))
            {
                switch (attributeOperator)
                {
                    // Números não usam StartsWith
                    case AttributeOperator.StartsWith:
                    case AttributeOperator.Equals:
                        value1Constant = Expression.Constant(Convert.ToInt32(attributeFilter.Value1), typeof(Int32));
                        valueExpression = Expression.Equal(idExpressionProperty, value1Constant);
                        break;
                    case AttributeOperator.LessThan:
                        value1Constant = Expression.Constant(Convert.ToInt32(attributeFilter.Value1), typeof(Int32));
                        valueExpression = Expression.LessThan(idExpressionProperty, value1Constant);
                        break;
                    case AttributeOperator.GraterThan:
                        value1Constant = Expression.Constant(Convert.ToInt32(attributeFilter.Value1), typeof(Int32));
                        valueExpression = Expression.GreaterThan(idExpressionProperty, value1Constant);
                        break;
                    case AttributeOperator.Between:
                        // VALUE >= VALUE1 AND VALUE <= VALUE2
                        value1Constant = Expression.Constant(Convert.ToInt32(attributeFilter.Value1), typeof(Int32));
                        var value2Constant = Expression.Constant(Convert.ToInt32(attributeFilter.Value2), typeof(Int32));
                        var value1GreaterThan = Expression.GreaterThanOrEqual(idExpressionProperty, value1Constant);
                        var value2LessThan = Expression.LessThanOrEqual(idExpressionProperty, value2Constant);
                        valueExpression = Expression.And(value1GreaterThan, value2LessThan);
                        break;
                    case AttributeOperator.LessThanOrEqual:
                        value1Constant = Expression.Constant(Convert.ToInt32(attributeFilter.Value1), typeof(Int32));
                        valueExpression = Expression.LessThanOrEqual(idExpressionProperty, value1Constant);
                        break;
                    case AttributeOperator.GraterThanOrEqual:
                        value1Constant = Expression.Constant(Convert.ToInt32(attributeFilter.Value1), typeof(Int32));
                        valueExpression = Expression.GreaterThanOrEqual(idExpressionProperty, value1Constant);
                        break;
                    case AttributeOperator.Contains:
                        List<Int32> values = new List<int>();

                        // Vamos converter os valores
                        foreach (var value in ((String)attributeFilter.Value1).Split(';'))
                        {
                            values.Add(Convert.ToInt32(value));
                        }
                        value1Constant = Expression.Constant(values.ToArray(), typeof(Int32[]));
                        var containsMethods = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(aaa => aaa.Name == "Contains");
                        MethodInfo methodInfo = null;
                        foreach (var method in containsMethods)
                        {
                            if (method.GetParameters().Count() == 2)
                            {
                                methodInfo = method;
                                break;
                            }
                        }
                        methodInfo = methodInfo.MakeGenericMethod(idExpressionProperty.Type);
                        valueExpression = Expression.Call(methodInfo, new Expression[] { value1Constant, idExpressionProperty });
                        break;
                    default:
                        break;
                }
            }
            Expression finalWhereExpression = valueExpression;
            IQueryable<EntityTable> entityQueryable = context.EntityTable.Include(Constants.ENTITYATTRIBUTETABLE).AsQueryable();

            MethodCallExpression whereCallExpression = Expression.Call(typeof(Queryable),
                Constants.WHERE,
                new Type[] { entityQueryable.ElementType },
                entityQueryable.Expression,
                Expression.Lambda<Func<EntityTable, bool>>(
                    finalWhereExpression,
                    new ParameterExpression[] { entityTableExpressionParameter }
                ));

            return entityQueryable.Provider.CreateQuery<EntityTable>(whereCallExpression);
        }

        #endregion

        public void GetById(int idSystem, int idOrganization, int id, GetByIdCallback getByIdCallback)
        {
            try
            {
                using (MySqlEntityDatabaseContext db = new MySqlEntityDatabaseContext())
                {
                    // Recupera a entidade pelo id
                    var entity = (from a in db.EntityTable
                                  where
                                    (a.Id == id &&
                                    a.IdSystem == idSystem &&
                                    a.IdOrganization == idOrganization)
                                  select new Model.Entity()
                                  {
                                      IdSystem = a.IdSystem,
                                      IdOrganization = a.IdOrganization,
                                      Id = a.Id,
                                      Code = a.Code,
                                      Name = a.Name
                                  }).FirstOrDefault();

                    var entityAttributes = (from a in db.EntityAttributeTable
                                            where
                                              (a.IdEntity == id)
                                            select a).ToList();

                    foreach (var attribute in entityAttributes)
                    {
                        var type = (EntityAttributeType)attribute.Type;

                        entity.EntityAttributes.Add(new EntityAttribute()
                        {
                            Id = attribute.Id,
                            IdEntity = attribute.IdEntity,
                            Name = attribute.Name,
                            Type = type,
                            EntityAttributeValue = GetEntityAttributeValueDictionary[type].Invoke(attribute.Id)
                        });
                    }

                    if (entity != null)
                    {
                        // Chama o callback passando a entidade
                        getByIdCallback.Found.Invoke(idSystem, idOrganization, id, entity);
                    }
                    else
                    {
                        // Chama o callback de não encontrado 
                        getByIdCallback.NotFound.Invoke(idSystem, idOrganization, id);
                    }
                }
            }
            catch (Exception e)
            {
                getByIdCallback.Exception.Invoke(idSystem, idOrganization, id, e);
            }
        }

        public void GetByCode(int idSystem, int idOrganization, string code, GetByCodeCallback getByCodeCallback)
        {
            try
            {
                using (DAL.MySqlEntityDatabaseContext db = new DAL.MySqlEntityDatabaseContext())
                {
                    // Recupera a entidade pelo codigo
                    var entity = (from a in db.EntityTable
                                  where
                                    (a.Code == code &&
                                    a.IdSystem == idSystem &&
                                    a.IdOrganization == idOrganization)
                                  select new Model.Entity()
                                  {
                                      IdSystem = a.IdSystem,
                                      IdOrganization = a.IdOrganization,
                                      Id = a.Id,
                                      Code = a.Code,
                                      Name = a.Name
                                  }).FirstOrDefault();

                    var entityAttributes = (from a in db.EntityAttributeTable
                                            where
                                              (a.IdEntity == entity.Id)
                                            select a).ToList();

                    foreach (var attribute in entityAttributes)
                    {
                        var type = (EntityAttributeType)attribute.Type;

                        entity.EntityAttributes.Add(new EntityAttribute()
                        {
                            Id = attribute.Id,
                            IdEntity = attribute.IdEntity,
                            Name = attribute.Name,
                            Type = type,
                            EntityAttributeValue = GetEntityAttributeValueDictionary[type].Invoke(attribute.Id)
                        });
                    }

                    if (entity != null)
                    {
                        // Chama o callback passando a entidade
                        getByCodeCallback.Found.Invoke(idSystem, idOrganization, code, entity);
                    }
                    else
                    {
                        // Chama o callback de não encontrado 
                        getByCodeCallback.NotFound.Invoke(idSystem, idOrganization, code);
                    }
                }
            }
            catch (Exception e)
            {
                getByCodeCallback.Exception.Invoke(idSystem, idOrganization, code, e);
            }
        }

        public void GetByAttributes(int idSystem, int idOrganization, IList<IAttribute> attributes, GetByAttributesCallback getByAttributes)
        {
            try
            {
                using (MySqlEntityDatabaseContext db = new MySqlEntityDatabaseContext())
                {
                    // Entidades filtradas final
                    IQueryable<EntityTable> filteredEntities = null;

                    // FASE 1 -> Verificar quais as entidades que possuem os atributos que serão filtrados
                    IQueryable<EntityAttributeTable> finalEntityAttributesQuery = null;

                    // ----------------------------- filtrar por id

                    var atributoFiltroPorId = (from a in attributes
                                               where
                                                   a.Name.ToLower().Equals(Constants.ID.ToLower())
                                               select
                                                   a).FirstOrDefault();

                    IQueryable<EntityTable> consultarEntidades = null;

                    // Se tiver o tributo de pesquisa por id então substitui a consultarEntidades
                    if (atributoFiltroPorId != null)
                    {
                        // removemos o filtro por id da coleção para seguir lógica normal
                        attributes.Remove(atributoFiltroPorId);

                        // criamos uma consulta de entidades somente com os ids
                        consultarEntidades = GetFilteredEntitiesByIdQueryable(db, atributoFiltroPorId);
                    }
                    else
                    {
                        consultarEntidades = db.EntityTable.AsQueryable();
                    }

                    // Se existe alguma atributo para ser filtrado
                    if (attributes.Count > 0)
                    {
                        // Para cada atributo passado como parâmetro
                        foreach (var attribute in attributes)
                        {
                            // Recupera todas os atributos que foram filtrados por sistema, organização e nome
                            var entityAttributesQuery = from a in consultarEntidades
                                                        join b in db.EntityAttributeTable
                                                            on a.Id equals b.IdEntity
                                                        where
                                                          (a.IdSystem == idSystem &&
                                                          a.IdOrganization == idOrganization) &&
                                                          b.Name.ToLower() == attribute.Name.ToLower()
                                                        select b;

                            // A partir do segundo atributo de pesquisa em diante as consultas são concatenadas para executarem de uma vez só
                            if (finalEntityAttributesQuery != null)
                            {
                                finalEntityAttributesQuery = finalEntityAttributesQuery.Concat(entityAttributesQuery);
                            }
                            else
                            {
                                finalEntityAttributesQuery = entityAttributesQuery;
                            }
                        }

                        // Agrupa os atributos pelo id da entidade, contando quantos atributos foram encontrados com os parâmetros sistema, organização e nome
                        var entitiesGroupByIdEntity = from a in finalEntityAttributesQuery
                                                      group a by a.IdEntity into groupById
                                                      select new
                                                      {
                                                          IdEntity = groupById.Key,
                                                          Count = groupById.Count()
                                                      };

                        // Recupera as entidades que tem todos os atributos passados como parâmetro
                        var entitiesGroupThatHasAllAttributes = from a in entitiesGroupByIdEntity
                                                                where
                                                                  a.Count == attributes.Count
                                                                select a;

                        // Cria uma lista com os atributos cujos seus valores serão comparados com os parâmetros
                        IQueryable<EntityAttributeTable> atributosParaFiltrarValor =
                            from a in finalEntityAttributesQuery
                            join b in entitiesGroupThatHasAllAttributes
                                on a.IdEntity equals b.IdEntity
                            select a;

                        // FASE 2 - Para cada entidade da qual seus atributos conferem com os atributos da pesquisa 
                        // nós vamos comparar seus valores agora

                        // Armazena os ids das entidades que estão aptas para serem filtradas
                        Dictionary<Int32?, Boolean> filteredEntitiesDictionary = new Dictionary<int?, bool>();

                        foreach (var entityAttribute in atributosParaFiltrarValor.ToList())
                        {
                            var attributeValueParam = (from a in attributes
                                                       where
                                                            a.Name.ToLower() == entityAttribute.Name.ToLower()
                                                       select a).SingleOrDefault();

                            // Se não criou uma entrada de filtro para esse id de entidade
                            if (!filteredEntitiesDictionary.Keys.FirstOrDefault(x => x == entityAttribute.IdEntity).HasValue)
                            {
                                filteredEntitiesDictionary.Add(entityAttribute.IdEntity, true);
                            }

                            // True se o valor passou pelo filtro com sucesso
                            // Invoca a function configurada no dicionario
                            bool filterApplied = this.FilterEntityAttributeValueDictionary[(EntityAttributeType)entityAttribute.Type]
                                .Invoke(db, entityAttribute, attributeValueParam);

                            // Atualiza o dicionario de filtros das entidades 
                            filteredEntitiesDictionary[entityAttribute.IdEntity] = filteredEntitiesDictionary[entityAttribute.IdEntity] && filterApplied;
                        }

                        // FASE 3 - Vamos construir as expressões equals para cada id de entidade que foi filtrado

                        var entityParam = Expression.Parameter(typeof(EntityTable), Constants.ENTITY);
                        Expression entitiesWhereExpression = null;

                        // Lista das entidades que serão filtradas
                        var preFilteredEntities = filteredEntitiesDictionary.Where(x => x.Value);

                        if (preFilteredEntities.Count() > 0)
                        {
                            foreach (var idFiltrado in preFilteredEntities)
                            {
                                var idEntityProperty = Expression.Property(entityParam, typeof(EntityTable).GetProperty(Constants.ID));
                                var idEntityConstant = Expression.Constant(idFiltrado.Key, typeof(Int32));
                                var idExpression = Expression.Equal(idEntityProperty, idEntityConstant);

                                if (entitiesWhereExpression != null)
                                {
                                    entitiesWhereExpression = Expression.Or(entitiesWhereExpression, idExpression);
                                }
                                else
                                {
                                    entitiesWhereExpression = idExpression;
                                }
                            }

                            IQueryable<EntityTable> entityQueryable = db.EntityTable.Include(Constants.ENTITYATTRIBUTETABLE).AsQueryable();
                            MethodCallExpression whereCallExpression = Expression.Call(typeof(Queryable),
                                Constants.WHERE,
                                new Type[] { entityQueryable.ElementType },
                                entityQueryable.Expression,
                                Expression.Lambda<Func<EntityTable, bool>>(
                                entitiesWhereExpression,
                                new ParameterExpression[] { entityParam })
                            );

                            // Lista com as entidades que foram filtradas
                            filteredEntities = entityQueryable.Provider.CreateQuery<EntityTable>(whereCallExpression);
                        }

                    }  // Fim se tem atributos para filtrar
                    else
                    {
                        // Se não tem atributos para filtrar e existe um atributo de filtro por id, 
                        // então retorna as entidades com esses ids
                        if (atributoFiltroPorId != null)
                        {
                            filteredEntities = from e in consultarEntidades
                                               where
                                                    (e.IdSystem == idSystem &&
                                                     e.IdOrganization == idOrganization)
                                               select e;
                        }
                    }

                    // Se tiver o atributo de filtro volta ele na coleção para os callbacks votarem ele
                    if (atributoFiltroPorId != null)
                    {
                        attributes.Add(atributoFiltroPorId);
                    }

                    // ------------------------------- Criar entidades e retornar

                    // Se no final existe alguma entidade para retornar após aplicação dos filtros
                    if (filteredEntities != null)
                    {
                        // Entidades que serão retornadas
                        IList<IEntity> entitiesResult = new List<IEntity>();

                        // Para cada entidade filtrada vamos recuperar seus atributos e valores usando o GetById
                        foreach (var filteredEntity in filteredEntities)
                        {
                            // Recupera a entidade pelo id
                            var entity = new Model.Entity()
                            {
                                IdSystem = filteredEntity.IdSystem,
                                IdOrganization = filteredEntity.IdOrganization,
                                Id = filteredEntity.Id,
                                Code = filteredEntity.Code,
                                Name = filteredEntity.Name
                            };

                            foreach (var attribute in filteredEntity.EntityAttributeTable)
                            {
                                var type = (EntityAttributeType)attribute.Type;

                                entity.EntityAttributes.Add(new EntityAttribute()
                                {
                                    Id = attribute.Id,
                                    IdEntity = attribute.IdEntity,
                                    Name = attribute.Name,
                                    Type = type,
                                    EntityAttributeValue = GetEntityAttributeValueDictionary[type].Invoke(attribute.Id)
                                });
                            }

                            // adiciona a lista de retorno
                            entitiesResult.Add(entity);
                        }

                        if (entitiesResult.Count() > 0)
                        {
                            // Chama o callback passando a entidade
                            getByAttributes.Found.Invoke(idSystem, idOrganization, attributes, entitiesResult);
                        }
                        else
                        {
                            getByAttributes.NotFound.Invoke(idSystem, idOrganization, attributes);
                        }
                    }
                    else
                    {
                        getByAttributes.NotFound.Invoke(idSystem, idOrganization, attributes);
                    }
                }
            }
            catch (Exception e)
            {
                getByAttributes.Exception.Invoke(idSystem, idOrganization, attributes, e);
            }
        }

        public void Create(int idSystem, int idOrganization, IEntity entity, CreateCallback createCallback)
        {
            try
            {
                using (MySqlEntityDatabaseContext db = new MySqlEntityDatabaseContext())
                {
                    var transaction = db.StartReadCommittedTransaction();

                    // Criar nova entidade baseado na entidade enviada
                    EntityTable newEntity = new EntityTable()
                    {
                        IdSystem = idSystem,
                        IdOrganization = idOrganization,
                        Name = entity.Name,
                        Code = entity.Code ?? RNGKey.New() // Gera um código automático quando não passado um código
                    };

                    // Adcionar no EF
                    db.EntityTable.AddObject(newEntity);

                    // Adicionar cada atributo passado pela entidade parâmetro
                    foreach (var attributeToAdd in entity.EntityAttributes)
                    {
                        // Criar novo atributo da entidade
                        EntityAttributeTable newEntityAttribute = new EntityAttributeTable()
                        {
                            EntityTable = newEntity,
                            Name = attributeToAdd.Name,
                            Type = Convert.ToInt16(attributeToAdd.Type)
                        };

                        // Criar um valor para o atributo novo
                        this.NewEntityAttributeValueDictionary[attributeToAdd.Type.GetValueOrDefault()]
                            .Invoke(newEntityAttribute, attributeToAdd.EntityAttributeValue);
                    }

                    // Salvar no banco de dados
                    if (db.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave) > 0)
                    {
                        transaction.Commit(); // comita a transação

                        // Vamos chamar o GetById para trazer a entidade sem vínculo como contexto mysql
                        this.GetById(idSystem, idOrganization, newEntity.Id, new GetByIdCallback()
                        {
                            Found = (idSystem_, idOrganization_, id_, entity_) =>
                            {
                                // Retorna a entidade
                                createCallback.Created.Invoke(idSystem, idOrganization, entity_);
                            },
                            NotFound = (idSystem_, idOrganization_, id) =>
                            {
                                createCallback.NotCreated(idSystem_, idOrganization_, entity);
                            },
                            Exception = (idSystem_, idOrganization_, entity_, exception) =>
                            {
                                createCallback.Exception(idSystem_, idOrganization_, entity, exception);
                            }
                        });
                    }
                    else
                    {
                        transaction.Rollback(); // rollback na transação
                        createCallback.NotCreated.Invoke(idSystem, idOrganization, entity);
                    }
                }
            }
            catch (Exception e)
            {
                createCallback.Exception.Invoke(idSystem, idOrganization, entity, e);
            }
        }

        public void Update(int idSystem, int idOrganization, IEntity entity, UpdateCallback updateCallback)
        {
            try
            {
                using (MySqlEntityDatabaseContext db = new MySqlEntityDatabaseContext())
                {
                    var transaction = db.StartReadCommittedTransaction();

                    // Recupera entidade
                    var entityToUpdate = (from e in db.EntityTable
                                          where
                                            (e.Id == entity.Id &&
                                            e.IdSystem == idSystem &&
                                            e.IdOrganization == idOrganization)
                                          select e).SingleOrDefault();

                    // Se achou a entidade que será atualizada
                    if (entityToUpdate != null)
                    {
                        // Atualiza o nome
                        if (!String.IsNullOrEmpty(entity.Name))
                        {
                            entityToUpdate.Name = entity.Name;
                        }

                        // Atualiza o código
                        if (!String.IsNullOrEmpty(entity.Code))
                        {
                            entityToUpdate.Code = entity.Code;
                        }

                        // Para cada atributo passado pela entidade de parâmetro nós vamos
                        // ou atualizar o valor, ou criar um novo atributo para a entidade
                        foreach (var attributeFromUpdate in entity.EntityAttributes)
                        {
                            // Recupera o atributo por id ou por nome para ser atualizado
                            var entityAttributeToUpdate = (from ea in db.EntityAttributeTable
                                                           where
                                                            (ea.Id == attributeFromUpdate.Id && ea.IdEntity == entityToUpdate.Id) ||
                                                            (ea.Name == attributeFromUpdate.Name && ea.IdEntity == entityToUpdate.Id)
                                                           select ea).SingleOrDefault();

                            // Se o atributo for achado então atualizar ele
                            if (entityAttributeToUpdate != null)
                            {
                                // Se ainda é o mesmo tipo
                                if (((IEntityAttribute)entityAttributeToUpdate).Type == attributeFromUpdate.Type)
                                {
                                    // Atualizar com o valor novo
                                    entityAttributeToUpdate.EntityAttributeValue.Value = attributeFromUpdate.EntityAttributeValue.Value;
                                }
                                else
                                {
                                    // Remover o valor antigo do atributo
                                    db.DeleteObject(entityAttributeToUpdate.EntityAttributeValue);

                                    // Criar um novo valor para o atributo
                                    this.NewEntityAttributeValueDictionary[attributeFromUpdate.Type.GetValueOrDefault()]
                                        .Invoke(entityAttributeToUpdate, attributeFromUpdate.EntityAttributeValue);

                                    // Atualiza o novo tipo do atributo
                                    entityAttributeToUpdate.Type = Convert.ToInt16(attributeFromUpdate.Type);
                                }
                            }
                            else // Caso contrário cria um novo atributo para essa entidade
                            {
                                // Criar novo atributo da entidade
                                EntityAttributeTable newEntityAttribute = new EntityAttributeTable()
                                {
                                    EntityTable = entityToUpdate,
                                    Name = attributeFromUpdate.Name,
                                    Type = Convert.ToInt16(attributeFromUpdate.Type)
                                };

                                // Criar um valor para o atributo
                                this.NewEntityAttributeValueDictionary[attributeFromUpdate.Type.GetValueOrDefault()]
                                    .Invoke(newEntityAttribute, attributeFromUpdate.EntityAttributeValue);
                            }
                        }

                        // Salvar no banco de dados
                        if (db.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave) > 0)
                        {
                            transaction.Commit();

                            // Vamos chamar o GetById para trazer a entidade sem vínculo como contexto mysql
                            this.GetById(idSystem, idOrganization, entity.Id, new GetByIdCallback()
                            {
                                Found = (idSystem_, idOrganization_, id_, entity_) =>
                                {
                                    // Retorna a entidade
                                    updateCallback.Updated(idSystem, idOrganization, entity_);
                                },
                                NotFound = (idSystem_, idOrganization_, id) =>
                                {
                                    updateCallback.NotUpdated(idSystem_, idOrganization_, entity);
                                },
                                Exception = (idSystem_, idOrganization_, entity_, exception) =>
                                {
                                    updateCallback.Exception.Invoke(idSystem_, idOrganization_, entity, exception);
                                }
                            });
                        }
                        else
                        {
                            transaction.Rollback();
                            updateCallback.NotUpdated(idSystem, idOrganization, entity);
                        }
                    }
                    else // Entidade não encontrada
                    {
                        transaction.Rollback();
                        updateCallback.NotUpdated(idSystem, idOrganization, entity);
                    }
                }
            }
            catch (Exception e)
            {
                updateCallback.Exception.Invoke(idSystem, idOrganization, entity, e);
            }
        }

        public void Delete(int idSystem, int idOrganization, IEntity entity, DeleteCallback deleteCallback)
        {
            try
            {
                using (MySqlEntityDatabaseContext db = new MySqlEntityDatabaseContext())
                {
                    var transaction = db.StartReadCommittedTransaction();

                    // Recupera entidade
                    var entityToDelete = (from a in db.EntityTable
                                          where
                                            a.Id == entity.Id &&
                                            a.IdSystem == idSystem &&
                                            a.IdOrganization == idOrganization
                                          select a).SingleOrDefault();

                    // Se achou a entidade que será atualizada
                    if (entityToDelete != null)
                    {
                        // Recupera os atributos da entidade
                        var entityAttributes = (from a in db.EntityAttributeTable
                                                where
                                                  a.IdEntity == entityToDelete.Id
                                                select a).ToList();

                        // Para cada atributo passado pela entidade parâmetro
                        foreach (var attributeToDelete in entityAttributes)
                        {
                            // Remove o valor do atributo, pois não dá para remover em cascata
                            db.DeleteObject(attributeToDelete.EntityAttributeValue);

                            // Remove o atributo
                            db.EntityAttributeTable.DeleteObject(attributeToDelete);
                        }

                        // Remove a entidade. Atributos da entidade serão removidos em cascata
                        db.EntityTable.DeleteObject(entityToDelete);

                        // Salvar no banco de dados
                        if (db.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave) > 0)
                        {
                            transaction.Commit();
                            deleteCallback.Deleted.Invoke(idSystem, idOrganization, entity);
                        }
                        else
                        {
                            transaction.Rollback();
                            deleteCallback.NotDeleted.Invoke(idSystem, idOrganization, entity);
                        }
                    }
                    else // Entidade não encontrada
                    {
                        transaction.Rollback();
                        deleteCallback.NotDeleted.Invoke(idSystem, idOrganization, entity);
                    }
                }
            }
            catch (Exception e)
            {
                deleteCallback.Exception.Invoke(idSystem, idOrganization, entity, e);
            }
        }
    }
}
