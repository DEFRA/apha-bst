using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.DataAccess.Data;
using Apha.BST.DataAccess.Repositories;

namespace Apha.BST.DataAccess.UnitTests.TrainingRepositoryTest
{
    public class AbstractTrainingRepositoryTest(
        BstContext context,
        IQueryable<TraineeTrainer>? traineeTrainers = null,
        IQueryable<TrainerTraining>? trainerTrainings = null,
        IQueryable<TrainerHistory>? trainerHistories = null,
        IQueryable<TrainerTrained>? trainerTraineds = null,
        IQueryable<Training>? trainings = null,
        IQueryable<Persons>? persons = null
    ) : TrainingRepository(context)
    {
        private readonly IQueryable<TraineeTrainer>? _traineeTrainers = traineeTrainers;
        private readonly IQueryable<TrainerTraining>? _trainerTrainings = trainerTrainings;
        private readonly IQueryable<TrainerHistory>? _trainerHistories = trainerHistories;
        private readonly IQueryable<TrainerTrained>? _trainerTraineds = trainerTraineds;
        private readonly IQueryable<Training>? _trainings = trainings;
        private readonly IQueryable<Persons>? _persons = persons;

        protected override IQueryable<T> GetQueryableResultFor<T>(string sql, params object[] parameters)
        {
            if (typeof(T) == typeof(TraineeTrainer) && _traineeTrainers != null)
                return (IQueryable<T>)_traineeTrainers;
            if (typeof(T) == typeof(TrainerTraining) && _trainerTrainings != null)
                return (IQueryable<T>)_trainerTrainings;
            if (typeof(T) == typeof(TrainerHistory) && _trainerHistories != null)
                return (IQueryable<T>)_trainerHistories;
            if (typeof(T) == typeof(TrainerTrained) && _trainerTraineds != null)
                return (IQueryable<T>)_trainerTraineds;
            throw new NotImplementedException($"No override for type {typeof(T).Name}");
        }

        protected override IQueryable<T> GetDbSetFor<T>()
        {
            if (typeof(T) == typeof(Training) && _trainings != null)
                return (IQueryable<T>)_trainings;
            if (typeof(T) == typeof(Persons) && _persons != null)
                return (IQueryable<T>)_persons;
            throw new NotImplementedException($"No override for type {typeof(T).Name}");
        }
    }
}
