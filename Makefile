# Concise introduction to GNU Make:
# https://swcarpentry.github.io/make-novice/reference.html

name = icon
ise_name = ise_icon
lbnl_name = lbnl_icon

# Inspired by https://docs.docker.com/engine/reference/commandline/run/#add-entries-to-container-hosts-file---add-host
docker_ip = $(shell ip -4 addr show scope global dev docker0 | grep inet | awk '{print $$2}' | cut -d / -f 1)

DOCKER_COMPOSE = \
	docker-compose \
		--file docker-compose.common.yml \
		--file docker-compose.yml \
		--project-name ${name}
ise_docker_compose = \
	docker-compose \
		--file docker-compose.common.yml \
		--file docker-compose.ise.yml \
		--project-name ${ise_name}
lbnl_docker_compose = \
	docker-compose \
		--file docker-compose.common.yml \
		--file docker-compose.lbnl.yml \
		--project-name ${lbnl_name}

# Taken from https://www.client9.com/self-documenting-makefiles/
help : ## Print this help
	@awk -F ':|##' '/^[^\t].+?:.*?##/ {\
		printf "\033[36m%-30s\033[0m %s\n", $$1, $$NF \
	}' $(MAKEFILE_LIST)
.PHONY : help
.DEFAULT_GOAL := help

# ----------------------------- #
# Interface with Docker Compose #
# ----------------------------- #

name : ## Print value of variable `name`
	@echo ${name}
.PHONY : name

name-ise : ## Print value of variable `ise_name`
	@echo ${ise_name}
.PHONY : name-ise

name-lbnl : ## Print value of variable `lbnl_name`
	@echo ${lbnl_name}
.PHONY : name-ise

build : ## Build images
	DOCKER_IP=${docker_ip} \
		${DOCKER_COMPOSE} build \
		--build-arg GROUP_ID=$(shell id --group) \
		--build-arg USER_ID=$(shell id --user)
.PHONY : build

build-ise : DOCKER_COMPOSE = ${ise_docker_compose}
build-ise : build ## Build ISE images
.PHONY : build-ise

build-lbnl : DOCKER_COMPOSE = ${lbnl_docker_compose}
build-lbnl : build ## Build LBNL images
.PHONY : build-lbnl

remove : ## Remove stopped containers
	DOCKER_IP=${docker_ip} \
		${DOCKER_COMPOSE} rm
.PHONY : remove

remove-ise : DOCKER_COMPOSE = ${ise_docker_compose}
remove-ise : remove ## Remove stopped ISE containers
.PHONY : remove-ise

remove-lbnl : DOCKER_COMPOSE = ${lbnl_docker_compose}
remove-lbnl : remove ## Remove stopped LBNL containers
.PHONY : remove-lbnl

remove_data : ## Remove all data volumes
	docker volume rm \
		${name}_data \
		${ise_name}_data \
		${lbnl_name}_data

# TODO `docker-compose up` does not support `--user`, see https://github.com/docker/compose/issues/1532
up : ## (Re)create and start containers
	DOCKER_IP=${docker_ip} \
		${DOCKER_COMPOSE} up \
		--remove-orphans \
		--detach
.PHONY : up

up-ise : DOCKER_COMPOSE = ${ise_docker_compose}
up-ise : up ## (Re)create and start ISE containers
.PHONY : up-ise

up-lbnl : DOCKER_COMPOSE = ${lbnl_docker_compose}
up-lbnl : up ## (Re)create and start LBNL containers
.PHONY : up-lbnl

down : ## Stop containers and remove containers, networks, volumes, and images created by `up`
	DOCKER_IP=${docker_ip} \
		${DOCKER_COMPOSE} down
.PHONY : down

down-ise : DOCKER_COMPOSE = ${ise_docker_compose}
down-ise : down ## Stop ISE containers and remove containers, networks, volumes, and images created by `up-ise`
.PHONY : down-ise

down-lbnl : DOCKER_COMPOSE = ${lbnl_docker_compose}
down-lbnl : down ## Stop LBNL containers and remove containers, networks, volumes, and images created by `up-lbnl`
.PHONY : down-lbnl

restart : ## Restart all stopped and running containers
	DOCKER_IP=${docker_ip} \
		${DOCKER_COMPOSE} restart
.PHONY : restart

restart-ise : DOCKER_COMPOSE = ${ise_docker_compose}
restart-ise : restart ## Restart all stopped and running ISE containers
.PHONY : restart-ise

restart-lbnl : DOCKER_COMPOSE = ${lbnl_docker_compose}
restart-lbnl : restart ## Restart all stopped and running LBNL containers
.PHONY : restart-lbnl

logs : ## Follow logs
	DOCKER_IP=${docker_ip} \
		${DOCKER_COMPOSE} logs \
		--follow
.PHONY : logs

logs-ise : DOCKER_COMPOSE = ${ise_docker_compose}
logs-ise : logs ## Follow ISE logs
.PHONY : logs-ise

logs-lbnl : DOCKER_COMPOSE = ${lbnl_docker_compose}
logs-lbnl : logs ## Follow LBNL logs
.PHONY : logs-lbnl

runf : ## Run the one-time command `${COMMAND}` against a fresh `frontend` container
	DOCKER_IP=${docker_ip} \
		${DOCKER_COMPOSE} run \
		--user $(shell id --user):$(shell id --group) \
		frontend \
		${COMMAND}
.PHONY : runf

runb : ## Run the one-time command `${COMMAND}` against a fresh `backend` container
	DOCKER_IP=${docker_ip} \
		${DOCKER_COMPOSE} run \
		--user $(shell id --user):$(shell id --group) \
		backend \
		${COMMAND}
.PHONY : runb

shellf : COMMAND = ash
shellf : runf ## Enter shell in a fresh `frontend` container
.PHONY : shellf

shellb : COMMAND = ash
shellb : runb ## Enter shell in a fresh `backend` container
.PHONY : shellb

shellb-examples : COMMAND = bash -c "cd ./examples && bash"
shellb-examples : runb ## Enter Bourne-again shell, aka, bash, in a fresh `backend` container
.PHONY : shellb-examples

psql : ## Enter PostgreSQL interactive terminal in the running `database` container
	DOCKER_IP=${docker_ip} \
		${DOCKER_COMPOSE} exec \
		database \
		psql \
		--username postgres \
		--dbname icon_development
.PHONY : psql

# Inspired by https://stackoverflow.com/questions/55485511/how-to-run-dotnet-dev-certs-https-trust/59702094#59702094
# and https://superuser.com/questions/226192/avoid-password-prompt-for-keys-and-prompts-for-dn-information/226229#226229
# See also https://github.com/dotnet/aspnetcore/issues/7246#issuecomment-541201757
# and https://github.com/dotnet/runtime/issues/31237#issuecomment-544929504
generate-ssl-certificate : ## Generate SSL certificate
	DOCKER_IP=${docker_ip} \
		docker run \
		--user $(shell id --user):$(shell id --group) \
		--tty \
		--interactive \
		--mount type=bind,source="$(shell pwd)/ssl",target=/ssl \
		nginx \
		sh -c "openssl req -x509 -nodes -days 365 -newkey rsa:2048 -subj "/CN=localhost" -passout pass:password -keyout /ssl/localhost.key -out /ssl/localhost.crt -config /ssl/localhost.conf && openssl pkcs12 -export -out /ssl/localhost.pfx -inkey /ssl/localhost.key -in /ssl/localhost.crt && openssl verify -CAfile /ssl/localhost.crt /ssl/localhost.crt"

# Inspired by https://stackoverflow.com/questions/55485511/how-to-run-dotnet-dev-certs-https-trust/59702094#59702094
# See also https://github.com/dotnet/aspnetcore/issues/7246#issuecomment-541201757
# and https://github.com/dotnet/runtime/issues/31237#issuecomment-544929504
trust-ssl-certificate : ## Trust the generated SSL certificate
	sudo cp ./ssl/localhost.crt /usr/local/share/ca-certificates
	sudo update-ca-certificates
	cat /etc/ssl/certs/ca-certificates.crt
	cat ./ssl/localhost.crt
	sudo cat /etc/ssl/certs/localhost.pem
	openssl verify ./ssl/localhost.crt

# ------------------------------------------------ #
# Tasks to run, for example, in a Docker container #
# ------------------------------------------------ #

diagrams : ## Draw images from textual UML diagrams
	plantuml diagrams/*.uml
.PHONY : diagrams
