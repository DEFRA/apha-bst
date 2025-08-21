# Configuration using jenkins environment variables - updated sql folderfile names
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
	@$(SQLCMD) -d $(DB_NAME) -i 01-tables/modify_tables_post_restore.sql || exit 1
	@$(SQLCMD) -d $(DB_NAME) -i 04-procedures/sp_report_people.sql || exit 1
	@$(SQLCMD) -d $(DB_NAME) -i 04-procedures/sp_sites_add.sql || exit 1
	@$(SQLCMD) -d $(DB_NAME) -i 04-procedures/sp_site_trainee_get.sql || exit 1
	@$(SQLCMD) -d $(DB_NAME) -i 04-procedures/sp_trainee_update.sql || exit 1
	@$(SQLCMD) -d $(DB_NAME) -i 04-procedures/sp_trainer_has_trained.sql || exit 1
	@$(SQLCMD) -d $(DB_NAME) -i 04-procedures/sp_trainer_trainedby_trained.sql || exit 1
	@$(SQLCMD) -d $(DB_NAME) -i 04-procedures/sp_training_select.sql || exit 1
	@$(SQLCMD) -d $(DB_NAME) -i 04-procedures/sp_user_selectall.sql || exit 1
	@$(SQLCMD) -d $(DB_NAME) -i 99-direct-scripts/tblDataEntry_add_record.sql || exit 1
	@$(SQLCMD) -d $(DB_NAME) -i 99-direct-scripts/tblUser_add_record.sql || exit 1
	@echo "Post-deploy scripts completed."

# Help
help:
	@echo "  make post-deploy   - Run post-deployment SQL scripts"
