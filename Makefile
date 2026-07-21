BIN = bin/debug
PUB = bin/Release/net10.0/linux-x64/publish

VERSION = 0.0.1

SEPARATOR = --------------------------------

build:
	@mkdir -p $(BIN)
	@rm -rf ./$(BIN)/resources
	@cp -R ./resources ./$(BIN)
	dotnet build --output $(BIN)
	@echo $(SEPARATOR)

run:
	@dotnet ./$(BIN)/ptk.dll

test:
	@mkdir -p $(BIN)
	@rm -rf ./$(BIN)/resources
	@cp -R ./resources ./$(BIN)
	dotnet build --output $(BIN)
	@echo $(SEPARATOR)
	@dotnet ./$(BIN)/ptk.dll

publish:
	dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true
	@rm -rf ./$(PUB)/resources
	@cp -R ./resources ./$(PUB)
	@echo $(SEPARATOR)

clean:
	@rm -rf ./$(BIN)

purge:
	@rm -rf ./bin
