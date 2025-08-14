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
    @$(SQLCMD) -d $(DB_NAME) -i postdeploy/tables/modify-tables-post-restore.sql || exit 1
    @echo "Post-deploy scripts completed."

# Help
help:
    @echo "  make post-deploy   - Run post-deployment SQL scripts"
