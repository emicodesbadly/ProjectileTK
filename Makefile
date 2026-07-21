#can be linux-x64 OR win-x64
PLATFORM = linux-x64

BIN = bin/debug
PUB = bin/Release/net10.0/$(PLATFORM)/publish

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
	dotnet publish -c Release -r $(PLATFORM) -p:PublishSingleFile=true
	@rm -rf ./$(PUB)/resources
	@cp -R ./resources ./$(PUB)
	@echo $(SEPARATOR)

clean:
	@rm -rf ./$(BIN)

purge:
	@rm -rf ./bin
