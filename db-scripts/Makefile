# Configuration using jenkins environment variables
DB_SERVER ?= localhost
DB_NAME ?= bstdb
DB_USER ?= bstdev
DB_PASSWORD ?= $(if $(DB_PASSWORD),$(DB_PASSWORD),$(error DB_PASSWORD is not set))
SQLCMD=sqlcmd -S $(DB_SERVER) -U $(DB_USER) -P $(DB_PASSWORD)

# Default target
all: post-deploy

# Run post-deployment scripts
post-deploy:
    @echo "Running post-deploy scripts..."
    @$(SQLCMD) -d $(DB_NAME) -i postdeploy/01 tables/modify-tables-post-restore.sql || exit 1
    @$(SQLCMD) -d $(DB_NAME) -i postdeploy/04 Procedures/sp_Report_People.sql || exit 1
    @$(SQLCMD) -d $(DB_NAME) -i postdeploy/04 Procedures/sp_Sites_Add.sql || exit 1
    @$(SQLCMD) -d $(DB_NAME) -i postdeploy/04 Procedures/sp_Site_Trainee_Get.sql || exit 1
    @$(SQLCMD) -d $(DB_NAME) -i postdeploy/04 Procedures/sp_Trainee_Update.sql || exit 1
    @$(SQLCMD) -d $(DB_NAME) -i postdeploy/04 Procedures/sp_Trainer_Has_Trained.sql || exit 1
    @$(SQLCMD) -d $(DB_NAME) -i postdeploy/04 Procedures/sp_Trainer_TrainedBy_Trained.sql || exit 1
    @$(SQLCMD) -d $(DB_NAME) -i postdeploy/04 Procedures/sp_Training_Select.sql || exit 1
    @$(SQLCMD) -d $(DB_NAME) -i postdeploy/04 Procedures/sp_User_SelectAll.sql || exit 1
    @$(SQLCMD) -d $(DB_NAME) -i postdeploy/99 Direct Scripts in Sequence/tblDataEntry_AddRecord.sql || exit 1
    @$(SQLCMD) -d $(DB_NAME) -i postdeploy/99 Direct Scripts in Sequence/tblUser_AddRecord.sql || exit 1
    @echo "Post-deploy scripts completed."

# Help
help:
    @echo "  make post-deploy   - Run post-deployment SQL scripts"
